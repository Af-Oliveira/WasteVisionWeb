using System.Collections.Generic;
using System.Linq;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.RoboflowModels;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.Predictions
{
    public static class PredictionMapper
    {
        public static PredictionDto ToDto(Prediction prediction)
        {
            if (prediction == null) return null;

            return new PredictionDto(
                prediction.Id.AsGuid(),
                prediction.UserId.AsString(),
                prediction.RoboflowModelId.AsString(),
                prediction.OriginalImageUrl.AsString(),
                prediction.ProcessedImageUrl.AsString(),
                prediction.Date.AsString() // ISO 8601 format
            );
        }

         public static PredictionApiDto ToApiDto(Prediction prediction)
        {
            if (prediction == null) return null;

            return new PredictionApiDto(
                prediction.Id.AsGuid(),
                prediction.UserId.AsString(),
                prediction.User.Username.AsString(),
                prediction.RoboflowModelId.AsString(),
                prediction.RoboflowModel.Description.AsString(),
                prediction.OriginalImageUrl.AsString(),
                prediction.ProcessedImageUrl.AsString(),
                prediction.Date.AsString() // ISO 8601 format
            );
        }

        public static List<PredictionDto> ToDtoList(List<Prediction> predictions)
        {
            if (predictions == null) return null;
            return predictions.Select(pred => ToDto(pred)).ToList();
        }

          public static List<PredictionApiDto> ToApiDtoList(List<Prediction> predictions)
        {
            if (predictions == null) return null;
            return predictions.Select(pred => ToApiDto(pred)).ToList();
        }

        public static Prediction ToDomain(CreatingPredictionDto dto)
        {
            if (dto == null) return null;

            return new Prediction(
                new UserId(dto.UserId),
                new RoboflowModelId(dto.ModelId),
                new Url(dto.OriginalImageUrl),
                new Url(dto.ProcessedImageUrl)
            );
        }
    }
}