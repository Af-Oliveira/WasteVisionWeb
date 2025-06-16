using DDDSample1.Domain.Users;

namespace DDDSample1.Domain.Application
{
    public static class ProfileMapper
    {
        public static ProfileDto ToDto(UserApiDto user)
        {
            if (user == null)
                return null;

            var roleDto = new RoleDto(user.RoleId, user.RoleName);
            var profileDto = new ProfileDto(user.Id, user.Email,user.Username, roleDto, user.Active);
            return profileDto;
        }
    }
}

