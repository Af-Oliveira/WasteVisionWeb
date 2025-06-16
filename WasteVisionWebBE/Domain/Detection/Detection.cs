// filepath: Detection.cs
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers; // Required for MediaTypeHeaderValue
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DDDSample1.Domain.Logging;

namespace DDDSample1.Domain.Detections
{
    public class Detection : IDetection
    {
        private readonly HttpClient _httpClient;
        private readonly ILogManager _logManager;
        private readonly string _flaskApiUrl;
        private readonly TimeSpan _roboflowTimeout = TimeSpan.FromSeconds(10);
        private readonly TimeSpan _flaskTimeout = TimeSpan.FromSeconds(30);

        public Detection(
            IHttpClientFactory httpClientFactory,
            ILogManager logManager)
        {
            _httpClient = httpClientFactory.CreateClient("DetectionClient");
            _logManager = logManager ?? throw new ArgumentNullException(nameof(logManager));
            _flaskApiUrl = "http://localhost:5000"; // Configure as needed
        }

        public async Task<RoboflowPredictionResponseDTO?> DetectObjectsAsync(
            byte[] imageBytes,
            string localModel, // This is the path to the model for Flask
            string modelUrl,
            string apiKey)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                _logManager.Write(LogType.Error, "DetectObjectsAsync called with null or empty imageBytes.");
                return null;
            }

            // Try Roboflow API first
            var roboflowResult = await TryRoboflowDetectionAsync(imageBytes, modelUrl, apiKey);
            if (roboflowResult != null)
            {
                return roboflowResult;
            }

            // Fallback to Flask API using multipart/form-data
            _logManager.Write(LogType.Detection, "Falling back to Flask YOLO API (multipart/form-data)");
            return await TryFlaskDetectionAsync(imageBytes, localModel);
        }

        private async Task<RoboflowPredictionResponseDTO?> TryRoboflowDetectionAsync(
            byte[] imageBytes,
            string modelUrl,
            string apiKey)
        {
            var requestUrl = $"{modelUrl}?api_key={apiKey}";
            var base64Image = Convert.ToBase64String(imageBytes);

            _logManager.Write(LogType.Detection, $"Sending detection request to Roboflow: {requestUrl}");

            try
            {
                using var cts = new CancellationTokenSource(_roboflowTimeout);
                using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                request.Content = new StringContent(base64Image, Encoding.UTF8, "application/x-www-form-urlencoded");

                using var response = await _httpClient.SendAsync(request, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<RoboflowPredictionResponseDTO>(jsonResponse, options);

                    if (result?.Predictions != null)
                    {
                        _logManager.Write(LogType.Detection, $"Roboflow detected {result.Predictions.Count} objects");
                    }
                    return result;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logManager.Write(LogType.Error, $"Roboflow API failed: {errorContent}");
                    return null;
                }
            }
            catch (OperationCanceledException)
            {
                _logManager.Write(LogType.Error, "Roboflow API timed out");
                return null;
            }
            catch (Exception ex)
            {
                _logManager.Write(LogType.Error, $"Roboflow API error: {ex.Message}");
                return null;
            }
        }

        private async Task<RoboflowPredictionResponseDTO?> TryFlaskDetectionAsync(byte[] imageBytes, string localModelPath)
        {
            if (string.Equals(localModelPath, "None", StringComparison.OrdinalIgnoreCase) || 
                string.Equals(localModelPath, "N/A", StringComparison.OrdinalIgnoreCase)) 
            {
                _logManager.Write(LogType.Error, "Flask detection called with null or empty localModelPath.");
                return null; // Or throw an ArgumentException if this is an unrecoverable state
            }



            _logManager.Write(LogType.Detection, $"Sending detection request to Flask API: {_flaskApiUrl}/detect with model path '{localModelPath}' as multipart/form-data");
            try
            {
                using var cts = new CancellationTokenSource(_flaskTimeout);
                using var content = new MultipartFormDataContent();

                // Image file part
                using var fileContent = new ByteArrayContent(imageBytes);
                // Ensure the content type is set, e.g., image/jpeg or image/png
                // For simplicity, using image/jpeg. Adjust if you know the exact type or make it dynamic.
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                // The "name" parameter ("file") must match what Flask expects in request.files['file']
                content.Add(fileContent, "file", "image.jpg"); // "image.jpg" is a placeholder filename

                // Model path part
                // The "name" parameter ("model_path") must match what Flask expects in request.form['model_path']
                content.Add(new StringContent(localModelPath), "model_path");

                using var response = await _httpClient.PostAsync($"{_flaskApiUrl}/detect", content, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<RoboflowPredictionResponseDTO>(jsonResponse, options);

                    if (result?.Predictions != null)
                    {
                        _logManager.Write(LogType.Detection, $"Flask YOLO (model: {localModelPath}) detected {result.Predictions.Count} objects");
                    }
                    return result;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logManager.Write(LogType.Error, $"Flask API request failed (model: {localModelPath}): {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (OperationCanceledException)
            {
                _logManager.Write(LogType.Error, "Flask API request timed out");
                return null;
            }
            catch (HttpRequestException ex)
            {
                _logManager.Write(LogType.Error, $"Flask API HTTP request error: {ex.Message} (Inner: {ex.InnerException?.Message})");
                return null;
            }
            catch (Exception ex)
            {
                _logManager.Write(LogType.Error, $"Error during Flask API call: {ex.Message}");
                return null;
            }
        }
    }
}
