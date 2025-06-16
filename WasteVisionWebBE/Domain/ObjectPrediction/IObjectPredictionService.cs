using System.Threading.Tasks;
using System.Collections.Generic;
using DDDSample1.Domain.Predictions;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.ObjectPredictions
{
    public interface IObjectPredictionService
    {
        Task<List<ObjectPredictionDto>> GetAllAsync(ObjectPredictionSearchParamsDto searchParams);
        Task<ObjectPredictionDto> GetByIdAsync(ObjectPredictionId id);
        Task<List<ObjectPredictionDto>> GetByPredictionIdAsync(PredictionId predictionId);
        Task<ObjectPredictionDto> CreateAsync(CreatingObjectPredictionDto dto);
        Task<List<ObjectPredictionDto>> GetByCategoryAsync(Description category);

    }
}