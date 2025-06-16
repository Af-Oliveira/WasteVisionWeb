using DDDSample1.Domain.Detections;

namespace DDDSample1.Domain.Predictions
{
    public class CreatingPredictionDto
    {
        // Public properties for JSON deserialization
        public string UserId { get; set; }
        public string ModelId { get; set; }
        public string OriginalImageUrl { get; set; }
        public string ProcessedImageUrl { get; set; }
        public RoboflowPredictionResponseDTO ObjectPrediction { get; set; }

        public class Builder
        {
            private readonly CreatingPredictionDto dto;

            public Builder()
            {
                dto = new CreatingPredictionDto();
            }

            public Builder WithUserId(string userId)
            {
                dto.UserId = userId;
                return this;
            }

            public Builder WithModelId(string modelId)
            {
                dto.ModelId = modelId;
                return this;
            }

            public Builder WithOriginalImageUrl(string originalImageUrl)
            {
                dto.OriginalImageUrl = originalImageUrl;
                return this;
            }

            public Builder WithProcessedImageUrl(string processedImageUrl)
            {
                dto.ProcessedImageUrl = processedImageUrl;
                return this;
            }

            public Builder WithObjectPrediction(RoboflowPredictionResponseDTO objectPrediction)
            {
                dto.ObjectPrediction = objectPrediction;
                return this;
            }

            public CreatingPredictionDto Build()
            {
                return dto;
            }
        }
    }
}