using System.Collections.Generic;
using System.Linq;
using DDDSample1.Domain.Roles;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.Users
{
    public static class UserMapper
    {
        public static UserDto ToDto(User user)
        {
            if (user == null)
                return null;

            return new UserDto(
                user.Id.AsString(),
                user.Email.AsString(),
                user.Username.AsString(),
                user.RoleId
            );
        }

        public static UserApiDto ToApiDto(User user)
        {
            if (user == null)
                return null;

            return new UserApiDto(
                user.Id.AsString(),
                user.Email.AsString(),
                user.Username.AsString(),
                user.Role?.Description?.AsString(),
                user.Role?.Id?.AsString(),
                user.Active
            );
        }

        public static User ToDomain(CreatingUserDto dto)
        {
            if (dto == null)
                return null;

            return new User(
                new Email(dto.Email),
                new Username(dto.Username),
                new RoleId(dto.RoleId)
            );
        }

        public static List<UserDto> ToDtoList(List<User> userList)
        {
            return userList.Select(user => ToDto(user)).ToList();
        }

        public static List<UserApiDto> ToApiDtoList(List<User> userList)
        {
            return userList.Select(user => ToApiDto(user)).ToList();
        }
    }
}
