using DDDSample1.Domain.Roles;

namespace DDDSample1.Domain.Users
{
    public class CreatingUserDto
    {
        // Public properties for JSON deserialization
        public string Email { get; set; }
        public string Username { get; set; }
        public string RoleId { get; set; }

        public CreatingUserDto()
        {
        }
        public CreatingUserDto(string email, string username, string roleId)
        {
            Email = email;
            Username = username;
            RoleId = roleId;
        }
    }
}
