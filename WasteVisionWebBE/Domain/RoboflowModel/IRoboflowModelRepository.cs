using DDDSample1.Domain.Shared;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DDDSample1.Domain.RoboflowModels
{
    public interface IRoboflowModelRepository : IRepository<RoboflowModel, RoboflowModelId>
    {

        Task<List<RoboflowModel>> GetAllActiveAsync();
        Task<List<RoboflowModel>> GetAllWithFiltersAsync(RoboflowModelSearchParamsDto searchParams);
    }
}