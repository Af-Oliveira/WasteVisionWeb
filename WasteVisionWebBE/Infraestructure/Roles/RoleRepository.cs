using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.Roles;
using DDDSample1.Domain.Shared;
using DDDSample1.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace DDDSample1.Infrastructure.Roles
{
    public class RoleRepository : BaseRepository<Role, RoleId>, IRoleRepository
    {

        private readonly DDDSample1DbContext _context;

        public RoleRepository(DDDSample1DbContext context) : base(context.Roles)
        {
            _context = context;
        }

        public async Task<Role> GetRoleByDescriptionAsync(Description description)
        {
            var role = await _context.Roles.FirstAsync(r => r.Description == description);
            return role;
        }

        public async Task<List<Role>> GetAllWithFiltersAsync(RoleSearchParamsDto searchParams)
        {
            var roleList = await _context.Roles
                .ToListAsync();

            var filteredRoles = roleList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchParams.Active))
            {
                bool isActive = searchParams.Active == "1";
                filteredRoles = filteredRoles.Where(s => s.Active == isActive);
            }

            if (!string.IsNullOrWhiteSpace(searchParams.Description))
                filteredRoles = filteredRoles.Where(s => s.Description.Value.Contains(searchParams.Description));

            return filteredRoles.ToList();
        }
    }
}