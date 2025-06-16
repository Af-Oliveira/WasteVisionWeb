using System;
using System.Collections.Generic;
using System.Linq;
using DDDSample1.Domain.Roles;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.RoboflowModels
{
    public static class RoboflowModelMapper
    {
        public static RoboflowModelDto ToDto(RoboflowModel model)
        {
            if (model == null)
                return null;

            return new RoboflowModelDto(
                model.Id.AsGuid(),
                model.Description.AsString(),
                model.ApiKey.AsString(),
                model.ModelUrl.AsString(),
                model.LocalModelPath.AsString(),
                model.EndPoint.AsString(),
                model.Map.AsString(),
                model.Recall.AsString(),
                model.Precision.AsString(),
                model.Active
            );
        }

        public static RoboflowModel ToDomain(CreatingRoboflowModelDto dto)
        {
            if (dto == null)
                return null;

            return new RoboflowModel(
                new ApiKey(dto.ApiKey),
                new Url(dto.ModelUrl),
                new FilePath(dto.LocalModelPath),
                new Description(dto.Description),
                new Url(dto.EndPoint),
                new NumberDouble(dto.Map),
                new NumberDouble(dto.Recall),
                new NumberDouble(dto.Precision)
            );
        }

        public static List<RoboflowModelDto> ToDtoList(List<RoboflowModel> models)
        {
            if (models == null)
                return null;

            return models.Select(model => ToDto(model)).ToList();
        }

       
    }
}