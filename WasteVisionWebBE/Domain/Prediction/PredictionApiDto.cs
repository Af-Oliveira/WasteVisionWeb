using System;

namespace DDDSample1.Domain.Predictions
{
    public class PredictionApiDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ModelId { get; set; }
        public string ModelName { get; set; }
        public string OriginalImageUrl { get; set; }
        public string ProcessedImageUrl { get; set; }
        public string Date { get; set; }

        public PredictionApiDto(Guid id, string userId, string userName, string modelId,string modelName,
            string originalImageUrl, string processedImageUrl,
            string date)
        {
            Id = id;
            UserId = userId;
            ModelId = modelId;
            UserName = userName;
            ModelName = modelName;
            OriginalImageUrl = originalImageUrl;
            ProcessedImageUrl = processedImageUrl;
            Date = date;

        }
    }
}