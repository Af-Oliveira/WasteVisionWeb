using System;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Predictions;

namespace DDDSample1.Domain.ObjectPredictions
{
    public class ObjectPrediction : Entity<ObjectPredictionId>, IAggregateRoot
    {
        public PredictionId PredictionId { get; private set; }
        // Add navigation property
        public Prediction Prediction { get; private set; }
        public NumberDouble X { get; private set; }
        public NumberDouble Y { get; private set; }
        public NumberDouble Width { get; private set; }
        public NumberDouble Height { get; private set; }
        public Description Category { get; private set; }
        public NumberDouble Confidence { get; private set; }

        private ObjectPrediction() { } // For ORM

        public ObjectPrediction(PredictionId predictionId, NumberDouble x, NumberDouble y, 
            Description category, NumberDouble confidence, NumberDouble width, NumberDouble height)
        {
            Id = new ObjectPredictionId(Guid.NewGuid());
            PredictionId = predictionId;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Category = category;
            Confidence = confidence;
        }
    }
}