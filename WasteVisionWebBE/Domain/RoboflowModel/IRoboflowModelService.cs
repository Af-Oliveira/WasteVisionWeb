using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDDSample1.Domain.RoboflowModels
{
    public interface IRoboflowModelService
    {
        Task<List<RoboflowModelDto>> GetAllAsync();
        Task<RoboflowModelDto> GetByIdAsync(RoboflowModelId id);
        Task<RoboflowModelDto> CreateAsync(CreatingRoboflowModelDto dto);
        Task<RoboflowModelDto> UpdateAsync(string id, UpdatingRoboflowModelDto dto);
        Task<bool> DeleteAsync(RoboflowModelId id);
        Task<bool> ActivateAsync(RoboflowModelId id);
        Task<bool> DeactivateAsync(RoboflowModelId id);
        Task<List<RoboflowModelDto>> GetAllActiveAsync();

        Task<List<RoboflowModelDto>> GetAllWithFiltersAsync(RoboflowModelSearchParamsDto searchParams);
    }
}