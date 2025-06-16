using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Activation;

namespace DDDSample1.Domain.Users
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();
        Task<UserApiDto> GetByIdAsync(UserId id);
        Task<UserApiDto> CreateAsync(CreatingUserDto dto);
        Task<UserApiDto> UpdateAsync(string id, UpdatingUserDto updateDto);
        Task<UserApiDto> GetByEmailAsync(string email);
        Task<bool> ActivateUserAsync(string id);
        Task<bool> DeactivateUserAsync(string id);
        Task<UserApiDto> GetByEmailWithRoleAsync(string email);
        Task<bool> DeleteAsync(UserId userId);
        Task<List<UserApiDto>> GetAllWithFiltersAsync(UserSearchParamsDto searchParams);
    }
}
