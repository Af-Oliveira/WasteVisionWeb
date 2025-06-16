using Microsoft.AspNetCore.Http;

namespace DDDSample1.Domain.Detections
{
    public class CreatingDetectionDto
    {
        // Public properties for JSON deserialization
        public IFormFile File { get; set; }
        public string RoboflowModelId { get; set; }

        public class Builder
        {
            private readonly CreatingDetectionDto dto;

            public Builder()
            {
                dto = new CreatingDetectionDto();
            }

            public Builder WithFile(IFormFile file)
            {
                dto.File = file;
                return this;
            }

            public Builder WithRoboflowModelId(string roboflowModelId)
            {
                dto.RoboflowModelId = roboflowModelId;
                return this;
            }

            public CreatingDetectionDto Build()
            {
                return dto;
            }
        }
    }
}