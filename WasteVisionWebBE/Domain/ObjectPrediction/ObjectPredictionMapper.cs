using System.Collections.Generic;
using System.Linq;
using DDDSample1.Domain.Predictions;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.ObjectPredictions
{
    public static class ObjectPredictionMapper
    {
        public static ObjectPredictionDto ToDto(ObjectPrediction obj)
        {
            if (obj == null) return null;

            return new ObjectPredictionDto(
                obj.Id.AsGuid(),
                obj.PredictionId.AsString(),
                obj.X.AsString(),
                obj.Y.AsString(),
                obj.Category.AsString(),
                obj.Confidence.AsString(),
                obj.Width.AsString(),
                obj.Height.AsString()
            );
        }

        public static List<ObjectPredictionDto> ToDtoList(List<ObjectPrediction> objs)
        {
            if (objs == null) return null;
            return objs.Select(o => ToDto(o)).ToList();
        }

        public static ObjectPrediction ToDomain(CreatingObjectPredictionDto dto)
        {
            if (dto == null) return null;

            return new ObjectPrediction(
                new PredictionId(dto.PredictionId),
                new NumberDouble(dto.X),
                new NumberDouble(dto.Y),
                new Description(dto.Category),
                new NumberDouble(dto.Confidence),
                new NumberDouble(dto.Width),
                new NumberDouble(dto.Height)
            );
        }
    }
}