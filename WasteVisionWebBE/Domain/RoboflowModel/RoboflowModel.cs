using System;
using System.Collections.Generic;
using DDDSample1.Domain.Predictions;

using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.RoboflowModels
{
    public class RoboflowModel : Entity<RoboflowModelId>, IAggregateRoot
    {
    
        public Description Description  { get; private set; }
        public ApiKey ApiKey { get; private set; }
        public Url ModelUrl { get; private set; }
        public FilePath LocalModelPath { get; private set; }
        public Url EndPoint { get; private set; }
        public NumberDouble Map { get; private set; }
        public NumberDouble Recall { get; private set; }
        public NumberDouble Precision { get; private set; }
        public bool Active { get; private set; }

        private RoboflowModel() { } // For ORM

        public RoboflowModel( ApiKey apiKey, Url modelUrl,FilePath localModelPath ,  Description description, Url endPoint, 
            NumberDouble map, NumberDouble recall, NumberDouble precision)
        {
            Id = new RoboflowModelId(Guid.NewGuid());
            ApiKey = apiKey;
            ModelUrl = modelUrl;
            LocalModelPath = localModelPath;
            Description = description;
            EndPoint = endPoint;
            Map = map;
            Recall = recall;
            Precision = precision;
            Active = true;
        }

        public void ChangeApiKey(ApiKey apiKey)
        {
            if (!Active) throw new BusinessRuleValidationException("Cannot change API key of an inactive model.");
            ApiKey = apiKey;
        }

        public void ChangeModelUrl(Url modelUrl)
        {
            if (!Active) throw new BusinessRuleValidationException("Cannot change model URL of an inactive model.");
            ModelUrl = modelUrl;
        }

        public void ChangeDescription(Description description)
        {
            if (!Active) throw new BusinessRuleValidationException("Cannot change description of an inactive model.");
            Description = description;
        }

        public void ChangeEndPoint(Url endPoint)
        {
            if (!Active) throw new BusinessRuleValidationException("Cannot change endpoint of an inactive model.");
            EndPoint = endPoint;
        }
        public void ChangeMap(NumberDouble map)
        {
            if (!Active) throw new BusinessRuleValidationException("Cannot change map of an inactive model.");
            Map = map;
        }
        public void ChangeRecall(NumberDouble recall)
        {
            if (!Active) throw new BusinessRuleValidationException("Cannot change recall of an inactive model.");
            Recall = recall;
        }
        public void ChangePrecision(NumberDouble precision)
        {
            if (!Active) throw new BusinessRuleValidationException("Cannot change precision of an inactive model.");
            Precision = precision;
        }

         public void ChangeLocalModelUrl(FilePath localModelPath)
        {
            if (!Active) throw new BusinessRuleValidationException("Cannot change the local model.");
            LocalModelPath = localModelPath;
        }


          public void Activate()
        {
            if (Active) throw new BusinessRuleValidationException("Roboflow model is already active.");
            Active = true;
        }

        public void Deactivate()
        {
            if (!Active) throw new BusinessRuleValidationException("Roboflow model is already inactive.");
            Active = false;
        }

    }
}