using System;

namespace DDDSample1.Domain.Application
{
    public class ProfileDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public RoleDto Role { get; set; }
        public bool? Active { get; set; }

        public ProfileDto(string Id, string Email, string Username, RoleDto Role, bool? Active)
        {
            this.Id = Id;
            this.Email = Email;
            this.Username = Username;
            this.Role = Role;
            this.Active = Active;
        }
    }

    public class RoleDto
    {
        public string Id { get; set; }
        public string Description { get; set; }

        public RoleDto(string Id, string Description)
        {
            this.Id = Id;
            this.Description = Description;
        }
    }
}
