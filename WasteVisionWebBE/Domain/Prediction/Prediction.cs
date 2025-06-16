using System;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.RoboflowModels;
using DDDSample1.Domain.ObjectPredictions;
using System.Collections.Generic;


namespace DDDSample1.Domain.Predictions
{
    public class Prediction : Entity<PredictionId>, IAggregateRoot
    {
        public UserId UserId { get; private set; }

        public User User { get; private set; }
        public RoboflowModelId RoboflowModelId { get; private set; }
        public RoboflowModel RoboflowModel { get; private set; }
        public Url OriginalImageUrl { get; private set; }
        public Url ProcessedImageUrl { get; private set; }
        public Date Date { get; private set; }

        public ICollection<ObjectPrediction> ObjectPredictions { get; private set; } = new List<ObjectPrediction>();

        private Prediction() { }

        public Prediction(UserId userId, RoboflowModelId roboflowModelIdId, Url originalImageUrl, Url processedImageUrl)
        {
            Id = new PredictionId(Guid.NewGuid());
            UserId = userId;
            RoboflowModelId = roboflowModelIdId;
            OriginalImageUrl = originalImageUrl;
            ProcessedImageUrl = processedImageUrl;
            Date = new Date(DateTime.UtcNow);
        }

    }
}