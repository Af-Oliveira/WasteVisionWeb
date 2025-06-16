using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDDSample1.Infrastructure.Users
{
    public class UserRepository : BaseRepository<User, UserId>, IUserRepository
    {
        private readonly DDDSample1DbContext _context;

        public UserRepository(DDDSample1DbContext context) : base(context.Users)
        {
            _context = context;
        }

        public async Task<User> GetByEmailAsync(Email email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public new async Task<User> GetByIdAsync(UserId id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<User> GetByEmailWithRoleAsync(Email email)
        {
            return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetAllWithFiltersAsync(UserSearchParamsDto searchParams)
        {
            var usersList = await _context.Users
                .Include(s => s.Role)
                .ToListAsync();

            var filteredUsers = usersList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchParams.Active))
            {
                bool isActive = searchParams.Active == "1";
                filteredUsers = filteredUsers.Where(s => s.Active == isActive);
            }

            if (!string.IsNullOrWhiteSpace(searchParams.Email))
                filteredUsers = filteredUsers.Where(s => s.Email.Value.Contains(searchParams.Email));

            if (!string.IsNullOrWhiteSpace(searchParams.Username))
                filteredUsers = filteredUsers.Where(s => s.Username.Value.Contains(searchParams.Username));


            if (!string.IsNullOrWhiteSpace(searchParams.RoleId))
                filteredUsers = filteredUsers.Where(s => s.Role.Id.Value.Contains(searchParams.RoleId));

            return filteredUsers.ToList();
        }
    }
}
