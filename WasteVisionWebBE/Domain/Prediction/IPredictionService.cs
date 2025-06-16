using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.RoboflowModels;

namespace DDDSample1.Domain.Predictions
{
    public interface IPredictionService
    {
        Task<List<PredictionDto>> GetAllAsync();
        Task<PredictionDto> GetByIdAsync(PredictionId id);
        Task<List<PredictionDto>> GetByUserIdAsync(UserId userId);
        Task<List<PredictionDto>> GetByModelIdAsync(RoboflowModelId modelId);
        Task<PredictionDto> CreateAsync(CreatingPredictionDto dto);
        Task<List<PredictionApiDto>> GetAllWithFiltersAsync(PredictionSearchParamsDto searchParams);
    }
}