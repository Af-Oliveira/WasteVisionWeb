// filepath: c:\Users\afons\Documents\Work\PESTI\WasteVisionWeb\WasteVisionWebBE\Domain\Detection\RoboflowDtos.cs
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DDDSample1.Domain.Detections
{
    public class RoboflowPredictionResponseDTO
    {
        [JsonPropertyName("predictions")]
        public List<RoboflowPredictionDTO> Predictions { get; set; } = new List<RoboflowPredictionDTO>();
    }

    public class RoboflowPredictionDTO
    {
        [JsonPropertyName("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        public double Y { get; set; }

        [JsonPropertyName("width")]
        public double Width { get; set; }

        [JsonPropertyName("height")]
        public double Height { get; set; }

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }

        [JsonPropertyName("class")]
        public string Class { get; set; } = string.Empty;

        [JsonPropertyName("class_id")]
        public int ClassId { get; set; }

        [JsonPropertyName("detection_id")] // Fixed typo
        public string DetectionId { get; set; } = string.Empty;
    }
}
