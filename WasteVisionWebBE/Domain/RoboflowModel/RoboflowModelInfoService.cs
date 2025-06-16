using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Logging;

namespace DDDSample1.Domain.RoboflowModels
{
    public class RoboflowModelInfoService : IRoboflowModelInfoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogManager _logManager;

        public RoboflowModelInfoService(IHttpClientFactory httpClientFactory, ILogManager logManager)
        {
            _httpClient = httpClientFactory.CreateClient("RoboflowClient");
            _logManager = logManager;
        }

        public async Task<RoboflowModelInfoDto> GetModelInfoAsync(string modelUrl, string apiKey)
        {
            try
            {
                var requestUrl = $"{modelUrl}?api_key={apiKey}";
                var response = await _httpClient.GetAsync(requestUrl);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logManager.Write(LogType.Error, $"Invalid API key or model URL: {modelUrl}");
                    throw new BusinessRuleValidationException("Invalid API key or model URL");
                }

                response.EnsureSuccessStatusCode();
                
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<RoboflowModelInfoDto>(jsonResponse, options);
            }
            catch (HttpRequestException ex)
            {
                _logManager.Write(LogType.Error, $"Failed to fetch model info: {ex.Message}");
                throw new BusinessRuleValidationException("Failed to validate model credentials");
            }
        }
    }
}