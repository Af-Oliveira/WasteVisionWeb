// filepath: DDDSample1/Domain/RoboflowModels/CreatingRoboflowModelDto.cs
using Microsoft.AspNetCore.Http; // Required for IFormFile

namespace DDDSample1.Domain.RoboflowModels
{
    public class CreatingRoboflowModelDto
    {
        // Public properties for JSON deserialization and form binding
        public string Description { get; set; }
        public string ApiKey { get; set; }
        public string ModelUrl { get; set; } // Roboflow API URL for fetching info
        
        // This will be populated by the service after fetching from Roboflow
        public string EndPoint { get; set; } 
        public string RoboflowId { get; set; } // Assuming this is also from Roboflow info
        public string Map { get; set; }
        public string Recall { get; set; }
        public string Precision { get; set; }

        // New property for the uploaded file
        public IFormFile ModelFile { get; set; } 

        // This will be populated by the service after saving the file
        // and will be used by the mapper.
        public string LocalModelPath { get; set; } 


        // Builder pattern might need adjustment if you use it extensively for IFormFile
        // For simplicity, direct property setting is often easier with IFormFile from a controller.
        // If you keep the builder, you'd add a WithModelFile method.
        public class Builder
        {
            private readonly CreatingRoboflowModelDto _dto;

            public Builder()
            {
                _dto = new CreatingRoboflowModelDto();
            }

            public Builder WithDescription(string description)
            {
                _dto.Description = description;
                return this;
            }

            public Builder WithApiKey(string apiKey)
            {
                _dto.ApiKey = apiKey;
                return this;
            }

            public Builder WithModelUrl(string modelUrl)
            {
                _dto.ModelUrl = modelUrl;
                return this;
            }

            public Builder WithEndPoint(string endPoint)
            {
                _dto.EndPoint = endPoint; // Typically set by service
                return this;
            }

            public Builder WithRoboflowId(string roboflowId)
            {
                _dto.RoboflowId = roboflowId; // Typically set by service
                return this;
            }

            public Builder WithMap(string map)
            {
                _dto.Map = map; // Typically set by service
                return this;
            }

            public Builder WithRecall(string recall)
            {
                _dto.Recall = recall; // Typically set by service
                return this;
            }

            public Builder WithPrecision(string precision)
            {
                _dto.Precision = precision; // Typically set by service
                return this;
            }

            public Builder WithModelFile(IFormFile modelFile)
            {
                _dto.ModelFile = modelFile;
                return this;
            }
            
            // This would be set by the service, not typically by the builder from client input
            public Builder WithLocalModelPath(string localModelPath)
            {
                _dto.LocalModelPath = localModelPath;
                return this;
            }

            public CreatingRoboflowModelDto Build()
            {
                return _dto;
            }
        }
    }
}
