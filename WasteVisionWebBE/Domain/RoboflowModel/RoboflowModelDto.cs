using System;

namespace DDDSample1.Domain.RoboflowModels
{
    public class RoboflowModelDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string ApiKey { get; set; }
        public string ModelUrl { get; set; }
        public string EndPoint { get; set; }
        public string LocalModelPath {  get; set;}
        public string Map { get; set; }
        public string Recall { get; set; }
        public string Precision { get; set; }
        public bool? Active { get; set; }

        public RoboflowModelDto(Guid id, string description, string apiKey, string modelUrl, string localModelPath,
            string endpoint, string map, string recall, string precision, bool? active)
        {
            Id = id;
            Description = description;
            ApiKey = apiKey;
            ModelUrl = modelUrl;
            LocalModelPath = localModelPath;
            EndPoint = endpoint;
            Map = map;
            Recall = recall;
            Precision = precision;
            Active = active;
        }
    }
}