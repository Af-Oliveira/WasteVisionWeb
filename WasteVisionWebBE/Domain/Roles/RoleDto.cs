using System;

namespace DDDSample1.Domain.Roles
{
    public class RoleDto
    {
        public Guid Id { get; set; }

        public string Description { get; set; }
        public bool? Active { get; set; }


        public RoleDto()
        {
        }

        public RoleDto(Guid id, string description, bool? active)
        {
            Id = id;
            Description = description;
            Active = active;
        }
    }
}