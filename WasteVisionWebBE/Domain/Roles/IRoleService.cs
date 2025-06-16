using System.Threading.Tasks;
using System.Collections.Generic;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.Roles
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAllWithFiltersAsync(RoleSearchParamsDto searchParams);
        Task<RoleDto> GetByIdAsync(RoleId id);
        Task<RoleDto> AddAsync(CreatingRoleDto dto);
        Task<RoleDto> UpdateAsync(string id, RoleDto dto);
        Task<bool> DeactivateAsync(RoleId id);
        Task<bool> ActivateAsync(RoleId id);
        Task<bool> DeleteAsync(RoleId id);
        Task<RoleDto> GetRoleByDescriptionAsync(Description description);
    }
}
