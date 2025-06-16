using System;

namespace DDDSample1.Domain.Predictions
{
    public class PredictionDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string ModelId { get; set; }
        public string OriginalImageUrl { get; set; }
        public string ProcessedImageUrl { get; set; }
        public string Date { get; set; }

        public PredictionDto(Guid id, string userId, string modelId,
            string originalImageUrl, string processedImageUrl,
            string date)
        {
            Id = id;
            UserId = userId;
            ModelId = modelId;
            OriginalImageUrl = originalImageUrl;
            ProcessedImageUrl = processedImageUrl;
            Date = date;

        }
    }
}