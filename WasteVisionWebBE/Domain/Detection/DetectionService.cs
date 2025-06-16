// filepath: c:\Users\afons\Documents\Work\PESTI\WasteVisionWeb\WasteVisionWebBE\Domain\Detectionion\Detectionservice.cs
using System.Threading.Tasks;
using System.Collections.Generic;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Logging;
using DDDSample1.Domain.Roles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using DDDSample1.Domain.RoboflowModels;

namespace DDDSample1.Domain.Detections
{
    public class DetectionService : IDetectionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IDetection _detection;
        private readonly IImageProcessorService _imageProcessorService;
        private readonly ILogManager _logManager;
        private readonly IRoboflowModelRepository _modelRepository;

        public DetectionService(
            IUnitOfWork unitOfWork,
            IWebHostEnvironment hostingEnvironment,
            IDetection detection,
            IImageProcessorService imageProcessorService,
            ILogManager logManager,
            IRoboflowModelRepository modelRepository)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            _detection = detection ?? throw new ArgumentNullException(nameof(detection));
            _imageProcessorService = imageProcessorService ?? throw new ArgumentNullException(nameof(imageProcessorService));
            _logManager = logManager ?? throw new ArgumentNullException(nameof(logManager));
            _modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
        }

        // Keep this public if direct upload is still needed, otherwise make private or remove
        public async Task<string> UploadDetectImageAsync(IFormFile file, string scheme, HostString host)
        {
            return await SaveImageAsync(file, scheme, host, "Detection_"); // Delegate to helper
        }

        // Renamed method to reflect the full process
        // Update the interface IDetectionService accordingly
        public async Task<ImageAnalysisResultDto> UploadDetectAndProcessAsync(IFormFile file, string modelId, string scheme, HostString host)
        {
            try
            {
                // Get RoboflowModel
                
                var model = await _modelRepository.GetByIdAsync(new RoboflowModelId(modelId));

                if (model == null)
                {
                    _logManager.Write(LogType.Error, $"RoboflowModel not found with ID: {modelId}");
                    throw new ArgumentException($"RoboflowModel not found with ID: {modelId}");
                }

                if (!model.Active)
                {
                    _logManager.Write(LogType.Error, $"RoboflowModel {modelId} is not active");
                    throw new InvalidOperationException($"RoboflowModel {modelId} is not active");
                }

                // Rest of your existing upload and process logic...
                string originalImageUrl = await SaveImageAsync(file, scheme, host, "Detection_");
                if (string.IsNullOrEmpty(originalImageUrl))
                {
                    _logManager.Write(LogType.Error, "Failed to save the original image.");
                    throw new ApplicationException("Failed to save the original image.");
                }
                _logManager.Write(LogType.Detection, $"Successfully uploaded image: {originalImageUrl}");

                // Step 2: Read Image Bytes
                byte[] imageBytes;
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        if (memoryStream.Length == 0)
                        {
                            _logManager.Write(LogType.Error, $"Empty file uploaded: {file.FileName}");
                            return new ImageAnalysisResultDto(originalImageUrl, null, null);
                        }
                        imageBytes = memoryStream.ToArray();
                    }
                    _logManager.Write(LogType.Detection, $"Successfully read {imageBytes.Length} bytes from file {file.FileName}");
                }
                catch (Exception ex)
                {
                    _logManager.Write(LogType.Error, $"Failed to read file: {file.FileName}. Error: {ex.Message}");
                    return new ImageAnalysisResultDto(originalImageUrl, null, null);
                }

                // Step 3: AI Detectionion
                RoboflowPredictionResponseDTO? predictions = null;
                try
                {

                    // Use model-specific URL and API key
                    predictions = await _detection.DetectObjectsAsync(
                        imageBytes,
                        model.LocalModelPath.AsString(),
                        model.EndPoint.AsString(), 
                        model.ApiKey.AsString()
                    );
                    if (predictions?.Predictions == null || predictions.Predictions.Count == 0)
                    {
                        _logManager.Write(LogType.Detection, $"No predictions found for image: {originalImageUrl}");
                        return new ImageAnalysisResultDto(originalImageUrl, predictions, null);
                    }
                    _logManager.Write(LogType.Detection, $"Successfully detected {predictions.Predictions.Count} objects in image");
                }
                catch (Exception ex)
                {
                    _logManager.Write(LogType.Error, $"AI Detection failed for image: {originalImageUrl}. Error: {ex.Message}");
                    return new ImageAnalysisResultDto(originalImageUrl, null, null);
                }

                // Step 4: Process Image
                byte[]? processedImageBytes = null;
                try
                {
                    processedImageBytes = await _imageProcessorService.DrawBoundingBoxesAsync(imageBytes, predictions.Predictions);
                    if (processedImageBytes == null)
                    {
                        _logManager.Write(LogType.Error, "Image processing failed - no output generated");
                        return new ImageAnalysisResultDto(originalImageUrl, predictions, null);
                    }
                    _logManager.Write(LogType.Detection, "Successfully processed image with bounding boxes");
                }
                catch (Exception ex)
                {
                    _logManager.Write(LogType.Error, $"Failed to process image with bounding boxes. Error: {ex.Message}");
                    return new ImageAnalysisResultDto(originalImageUrl, predictions, null);
                }

                // Step 5: Save Processed Image
                string? processedImageUrl = null;
                try
                {
                    var originalExtension = Path.GetExtension(file.FileName);
                    processedImageUrl = await SaveImageBytesAsync(processedImageBytes, originalExtension, scheme, host, "processed_");
                    if (string.IsNullOrEmpty(processedImageUrl))
                    {
                        _logManager.Write(LogType.Error, "Failed to save processed image");
                        return new ImageAnalysisResultDto(originalImageUrl, predictions, null);
                    }
                    _logManager.Write(LogType.Detection, $"Successfully saved processed image: {processedImageUrl}");
                }
                catch (Exception ex)
                {
                    _logManager.Write(LogType.Error, $"Failed to save processed image. Error: {ex.Message}");
                    return new ImageAnalysisResultDto(originalImageUrl, predictions, null);
                }

                _logManager.Write(LogType.Detection, "Completed full Detectionion and processing workflow successfully");
                return new ImageAnalysisResultDto(originalImageUrl, predictions, processedImageUrl);
            }
            catch (Exception ex)
            {
                _logManager.Write(LogType.Error, $"Unexpected error in Detectionion workflow: {ex.Message}");
                throw;
            }
        }

        // --- Helper Methods ---

        // Helper to save IFormFile
        private async Task<string> SaveImageAsync(IFormFile file, string scheme, HostString host, string prefix)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file provided or file is empty.");
            }
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }
            var extension = Path.GetExtension(file.FileName);
            return await SaveImageBytesAsync(fileBytes, extension, scheme, host, prefix);
        }

        // Helper to save byte[]
        private async Task<string> SaveImageBytesAsync(byte[] imageBytes, string fileExtension, string scheme, HostString host, string prefix)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                _logManager.Write(LogType.Error, "SaveImageBytesAsync called with empty byte array.");
                return null; // Or throw? Returning null might be safer depending on caller.
            }

            if (string.IsNullOrEmpty(_hostingEnvironment.WebRootPath))
            {
                _logManager.Write(LogType.Error, "WebRootPath is not configured.");
                throw new InvalidOperationException("Server configuration error: WebRootPath not found.");
            }

            string filePath = null;

            try
            {
                var uploadsFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                    _logManager.Write(LogType.Detection, $"Created directory: {uploadsFolderPath}");
                }

                var uniqueId = Guid.NewGuid().ToString();
                fileExtension = string.IsNullOrEmpty(fileExtension) ? ".jpg" : fileExtension.ToLowerInvariant(); // Default to jpg if no extension
                var newFileName = $"{prefix}{uniqueId}{fileExtension}";
                filePath = Path.Combine(uploadsFolderPath, newFileName);

                _logManager.Write(LogType.Detection, $"Attempting to save image bytes to: {filePath}");
                await File.WriteAllBytesAsync(filePath, imageBytes); // Use WriteAllBytesAsync for byte arrays
                _logManager.Write(LogType.Detection, $"Successfully saved image bytes to: {filePath}");

                var fileUrl = $"{scheme}://{host}/uploads/{newFileName}";
                return fileUrl;
            }
            catch (Exception ex)
            {
                _logManager.Write(LogType.Error, $"Error saving image bytes. TargetPath: {(filePath ?? "Unknown")}. Exception: {ex.Message}");
                // Re-throw as ApplicationException to be caught by controller
                throw new ApplicationException("An error occurred while saving image data.", ex);
            }
        }

        // --- Interface method update ---
        // Need to update IDetectionService to reflect the new method name and return type
        Task<ImageAnalysisResultDto> IDetectionService.UploadAndDetectAsync(IFormFile file, string modelId, string scheme, HostString host)
        {
            return UploadDetectAndProcessAsync(file, modelId, scheme, host);
        }
    }
}