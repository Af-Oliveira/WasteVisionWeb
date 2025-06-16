using System.Text.Json.Serialization;
using DDDSample1.Domain.Roles;

namespace DDDSample1.Domain.Users
{
    public class UserApiDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public bool? Active { get; set; }


        public UserApiDto()
        {
        }

        [JsonConstructor]
        public UserApiDto(string userId, string email, string username, string role, string roleId, bool? active)
        {
            Id = userId;
            Email = email;
            Username = username;
            RoleName = role;
            RoleId = roleId;
            Active = active;
        }
    }
}

