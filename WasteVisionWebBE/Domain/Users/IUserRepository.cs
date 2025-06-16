using DDDSample1.Domain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDDSample1.Domain.Users
{
    public interface IUserRepository : IRepository<User, UserId>
    {
        Task<User> GetByEmailAsync(Email email);
        Task<User> GetByEmailWithRoleAsync(Email email);
        Task<List<User>> GetAllWithFiltersAsync(UserSearchParamsDto searchParams);
    }
}
