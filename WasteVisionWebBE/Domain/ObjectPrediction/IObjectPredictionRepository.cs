using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Predictions;

namespace DDDSample1.Domain.ObjectPredictions
{
    public interface IObjectPredictionRepository : IRepository<ObjectPrediction, ObjectPredictionId>
    {
        Task<List<ObjectPrediction>> GetByPredictionIdAsync(PredictionId predictionId);
        Task<List<ObjectPrediction>> GetByCategoryAsync(Description category);
        Task<List<ObjectPrediction>> GetAllWithFiltersAsync(ObjectPredictionSearchParamsDto searchParams);

    }
}