using System;

namespace DDDSample1.Domain.ObjectPredictions
{
    public class CreatingObjectPredictionDto
    {
        // Public properties for JSON deserialization
        public string PredictionId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Category { get; set; }
        public double Confidence { get; set; }

        public class Builder
        {
            private readonly CreatingObjectPredictionDto dto;

            public Builder()
            {
                dto = new CreatingObjectPredictionDto();
            }

            public Builder WithPredictionId(string predictionId)
            {
                dto.PredictionId = predictionId;
                return this;
            }

            public Builder WithX(double x)
            {
                dto.X = x;
                return this;
            }

            public Builder WithY(double y)
            {
                dto.Y = y;
                return this;
            }

            public Builder WithWidth(double width)
            {
                dto.Width = width;
                return this;
            }

            public Builder WithHeight(double height)
            {
                dto.Height = height;
                return this;
            }

            public Builder WithCategory(string category)
            {
                dto.Category = category;
                return this;
            }

            public Builder WithConfidence(double confidence)
            {
                dto.Confidence = confidence;
                return this;
            }

            public CreatingObjectPredictionDto Build()
            {
                return dto;
            }

        }
    }
}