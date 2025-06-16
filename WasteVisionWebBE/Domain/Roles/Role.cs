using System;
using System.Collections.Generic;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.Roles
{
    public class Role : Entity<RoleId>, IAggregateRoot
    {

        public Description Description { get; private set; }

        public bool Active { get; private set; }

        private Role()
        {
            this.Active = true;
        }

        public Role(Description description)
        {
            this.Id = new RoleId(Guid.NewGuid());
            this.Description = description;
            this.Active = true;
        }

        public void ChangeDescription(Description description)
        {
            if (!this.Active)
                throw new BusinessRuleValidationException("It is not possible to change the description to an inactive role.");
            this.Description = description;
        }
        public void Deactivate()
        {
            Active = false;
        }

        public void Activate()
        {
            Active = true;
        }
    }
}