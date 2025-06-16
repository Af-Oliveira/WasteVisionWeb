namespace DDDSample1.Domain.Users
{
    public class UpdatingUserDto
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string RoleId { get; set; }

        public UpdatingUserDto()
        {
        }
        public UpdatingUserDto(string email, string username, string roleId)
        {
            Email = email;
            Username = username;
            RoleId = roleId;
        }
    }
}