using System.Threading.Tasks;
using System.Collections.Generic;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Logging;

namespace DDDSample1.Domain.Roles
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _repo;
        private readonly ILogManager _logManager;
        public RoleService(IUnitOfWork unitOfWork, IRoleRepository repo, ILogManager logManager)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _logManager = logManager;
        }

        public async Task<List<RoleDto>> GetAllWithFiltersAsync(RoleSearchParamsDto searchParams)
        {
            var list = await _repo.GetAllWithFiltersAsync(searchParams);

            return RoleMapper.ToDtoList(list);
        }

        public async Task<RoleDto> GetByIdAsync(RoleId id)
        {
            var role = await _repo.GetByIdAsync(id);

            if (role == null)
            {
                return null;
            }

            return RoleMapper.ToDto(role);
        }

        public async Task<RoleDto> AddAsync(CreatingRoleDto dto)
        {
            try
            {
                var role = new Role(new Description(dto.Description));
                await _repo.AddAsync(role);
                await _unitOfWork.CommitAsync();
                _logManager.Write(LogType.Role, $"Created role with description {role.Description.ToString()}");
                return RoleMapper.ToDto(role);
            }
            catch
            {
                _logManager.Write(LogType.Error, $"Failed to create role");
                throw;
            }

        }

        public async Task<RoleDto> UpdateAsync(string id, RoleDto dto)
        {
            var role = await _repo.GetByIdAsync(new RoleId(id));

            if (role == null)
            {
                _logManager.Write(LogType.Error, $"Failed to update role with description {role.Description.ToString()}");
                return null;
            }

            role.ChangeDescription(new Description(dto.Description));
            await _unitOfWork.CommitAsync();
            _logManager.Write(LogType.Role, $"Updated role with description {role.Description.ToString()}");
            return RoleMapper.ToDto(role);
        }

        public async Task<bool> DeactivateAsync(RoleId id)
        {
            var role = await _repo.GetByIdAsync(id);

            if (role == null)
            {
                _logManager.Write(LogType.Error, $"Failed to deactivate role with description {role.Description.ToString()}");
                return false;
            }

            role.Deactivate();
            await _unitOfWork.CommitAsync();
            _logManager.Write(LogType.Role, $"Deactivated role with description {role.Description.ToString()}");
            return true;
        }

        public async Task<bool> ActivateAsync(RoleId id)
        {
            var role = await _repo.GetByIdAsync(id);

            if (role == null)
            {
                _logManager.Write(LogType.Error, $"Failed to activate role");
                return false;
            }

            role.Activate();
            await _unitOfWork.CommitAsync();
            _logManager.Write(LogType.Role, $"Activated role with description {role.Description.ToString()}");
            return true;
        }

        public async Task<bool> DeleteAsync(RoleId id)
        {
            var role = await _repo.GetByIdAsync(id);
            if (role == null) return false;

            if (role.Active)
            {
                _logManager.Write(LogType.Error, $"Failed to delete role");
                throw new BusinessRuleValidationException("It is not possible to delete an active category.");
            }


            _repo.Remove(role);
            await _unitOfWork.CommitAsync();
            _logManager.Write(LogType.Role, $"Deleted role with description {role.Description.ToString()}");
            return true;
        }

        public async Task<RoleDto> GetRoleByDescriptionAsync(Description description)
        {
            var role = await _repo.GetRoleByDescriptionAsync(description);

            if (role == null)
                return null;

            return RoleMapper.ToDto(role);
        }
    }
}