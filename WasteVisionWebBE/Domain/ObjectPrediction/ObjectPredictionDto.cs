using System;

namespace DDDSample1.Domain.ObjectPredictions
{
    public class ObjectPredictionDto
    {
        public Guid Id { get; set; }
        public string PredictionId { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        
        public string Category { get; set; }
        public string Confidence { get; set; }


        public ObjectPredictionDto(Guid id, string predictionId, string x, string y,
            string category, string confidence, string width, string height)
        {
            Id = id;
            PredictionId = predictionId;
            X = x;
            Y = y;
            Category = category;
            Confidence = confidence;
            Width = width;
            Height = height;
        }
    }
}