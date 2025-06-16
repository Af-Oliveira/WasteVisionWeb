
using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.Roles
{
    public interface IRoleRepository : IRepository<Role, RoleId>
    {
        Task<Role> GetRoleByDescriptionAsync(Description description);
        Task<List<Role>> GetAllWithFiltersAsync(RoleSearchParamsDto searchParams);

    }
}