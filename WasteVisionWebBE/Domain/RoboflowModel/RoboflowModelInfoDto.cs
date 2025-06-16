using System.Text.Json.Serialization;

namespace DDDSample1.Domain.RoboflowModels
{
    public class RoboflowModelInfoDto
    {
        [JsonPropertyName("version")]
        public VersionInfo Version { get; set; }
    }

    public class VersionInfo
    {
        [JsonPropertyName("model")]
        public ModelInfo Model { get; set; }
    }

    public class ModelInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("endpoint")]
        public string Endpoint { get; set; }

        [JsonPropertyName("map")]
        public string Map { get; set; }

        [JsonPropertyName("recall")]
        public string Recall { get; set; }

        [JsonPropertyName("precision")]
        public string Precision { get; set; }
    }
}