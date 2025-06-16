using DDDSample1.Domain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.RoboflowModels;

namespace DDDSample1.Domain.Predictions
{
    public interface IPredictionRepository : IRepository<Prediction, PredictionId>
    {
        Task<List<Prediction>> GetByUserIdAsync(UserId userId);
        Task<List<Prediction>> GetByModelIdAsync(RoboflowModelId modelId);
        Task<List<Prediction>> GetAllWithFiltersAsync(PredictionSearchParamsDto searchParams);
    }
}