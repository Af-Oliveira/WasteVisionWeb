using System.Collections.Generic;
using System.Linq;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.Roles
{
    public static class RoleMapper
    {
        public static RoleDto ToDto(Role role)
        {
            if (role == null)
                return null;

            return new RoleDto
            {
                Id = role.Id.AsGuid(),
                Description = role.Description.AsString(),
                Active = role.Active
            };
        }

        public static Role ToDomain(CreatingRoleDto dto)
        {
            if (dto == null)
                return null;

            return new Role(
                new Description(dto.Description)
            );
        }

        public static List<RoleDto> ToDtoList(List<Role> roles)
        {
            if (roles == null)
                return null;

            return roles.Select(role => ToDto(role)).ToList();
        }

        public static void UpdateDomain(Role role, RoleDto dto)
        {
            if (role == null || dto == null)
                return;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                role.ChangeDescription(new Description(dto.Description));
        }
    }
}
