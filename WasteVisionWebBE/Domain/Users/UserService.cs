using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Roles;
using System.Collections.Generic;
using System;

using DDDSample1.Domain.Activation;
using Microsoft.AspNetCore.Http;

namespace DDDSample1.Domain.Users
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _repo;


        public UserService(IUnitOfWork unitOfWork, IUserRepository repo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var userList = await _repo.GetAllAsync();
            return UserMapper.ToDtoList(userList);
        }

        public async Task<UserApiDto> GetByIdAsync(UserId id)
        {
            var user = await _repo.GetByIdAsync(id);

            if (user == null)
                return null;

            return UserMapper.ToApiDto(user);
        }

        public async Task<UserApiDto> CreateAsync(CreatingUserDto dto)
        {
            try
            {
                var user = UserMapper.ToDomain(dto);
                await _repo.AddAsync(user);
                await _unitOfWork.CommitAsync();

                var createdUser = await _repo.GetByIdAsync(user.Id);


                return UserMapper.ToApiDto(createdUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }



        public async Task<UserApiDto> UpdateAsync(string id, UpdatingUserDto dto)
        {

            var user = await _repo.GetByIdAsync(new UserId(id));
            if (user == null)
            {
                return null;
            }

            var isModified = false;

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email.ToString())
            {
                user.ChangeEmail(new Email(dto.Email));
                isModified = true;
            }

            if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username != user.Username.ToString())
            {
                user.ChangeUsername(new Username(dto.Username));
                isModified = true;
            }

            if (!string.IsNullOrWhiteSpace(dto.RoleId) && dto.RoleId != user.RoleId.ToString())
            {
                user.ChangeRoleId(new RoleId(dto.RoleId));
                isModified = true;
            }

            if (isModified)
            {
                await _unitOfWork.CommitAsync();
            }
            return UserMapper.ToApiDto(user);
        }


        public async Task<UserApiDto> GetByEmailAsync(string email)
        {
            var user = await _repo.GetByEmailAsync(new Email(email));

            if (user == null)
                return null;

            return UserMapper.ToApiDto(user);
        }

        public async Task<bool> ActivateUserAsync(string id)
        {
            var user = await _repo.GetByIdAsync(new UserId(id));
            if (user == null)
            {
                return false;
            }

            user.Activate();
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> DeactivateUserAsync(string id)
        {
            var user = await _repo.GetByIdAsync(new UserId(id));
            if (user == null)
            {
                return false;
            }

            user.Deactivate();
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<UserApiDto> GetByEmailWithRoleAsync(string email)
        {
            var user = await _repo.GetByEmailWithRoleAsync(new Email(email));
            if (user == null)
            {
                return null;
            }

            return UserMapper.ToApiDto(user);
        }

        public async Task<bool> DeleteAsync(UserId userId)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            _repo.Remove(user);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<List<UserApiDto>> GetAllWithFiltersAsync(UserSearchParamsDto searchParams)
        {
            var userList = await _repo.GetAllWithFiltersAsync(searchParams);
            return UserMapper.ToApiDtoList(userList);
        }
    }
}
