using System.Text.Json.Serialization;
using DDDSample1.Domain.Roles;

namespace DDDSample1.Domain.Users
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string RoleId { get; set; }

        public UserDto()
        {
        }

        [JsonConstructor]
        public UserDto(string id, string email, string username, string roleId)
        {
            Id = id;
            Email = email;
            Username = username;
            RoleId = roleId;
        }

        public UserDto(string id, string email, string username, RoleId roleId)
        {
            Id = id;
            Email = email;
            Username = username;
            RoleId = roleId.Value;
        }
    }
}
