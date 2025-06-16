using System;
using DDDSample1.Domain.Roles;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.Users
{
    public class User : Entity<UserId>, IAggregateRoot
    {
        public Email Email { get; private set; }
        public Username Username { get; private set; }
        public RoleId RoleId { get; private set; }
        public Role Role { get; private set; }
        public bool Active { get; private set; }

        private User()
        {
            Active = false;
        } // For ORM

        public User(Email email, Username username, RoleId roleId)
        {
            Id = new UserId(Guid.NewGuid());
            Email = email;
            Username = username;
            RoleId = roleId;
            Active = false;
        }

        public void Activate()
        {
            if (Active) throw new BusinessRuleValidationException("User is already active.");
            Active = true;
        }

        public void Deactivate()
        {
            if (!Active) throw new BusinessRuleValidationException("User is already inactive.");
            Active = false;
        }

        public void ChangeEmail(Email newEmail)
        {
            if (!Active && !newEmail.Equals(Email) && Email.ToString() != Email.PLACE_HOLDER) throw new BusinessRuleValidationException("Cannot change email of an inactive user.");
            Email = newEmail;
        }

        public void ChangeUsername(Username newUsername)
        {
            if (!Active) throw new BusinessRuleValidationException("Cannot change username of an inactive user.");
            Username = newUsername;
        }

        public void ChangeRoleId(RoleId newRoleId)
        {
            if (!Active) throw new BusinessRuleValidationException("Cannot change role id of an inactive user.");
            RoleId = newRoleId;
        }
    }
}
