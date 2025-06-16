using System;
using Xunit;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.Roles;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Tests.Domain.Users
{
    public class UserTest
    {
        [Fact]
        public void TestUserCreation()
        {
            // Arrange
            var email = new Email("test@example.com");
            var username = new Username("testuser");
            var roleId = new RoleId(Guid.NewGuid());

            // Act
            var user = new User(email, username, roleId);

            // Assert
            Assert.NotNull(user.Id);
            Assert.Equal(email, user.Email);
            Assert.Equal(username, user.Username);
            Assert.Equal(roleId, user.RoleId);
            Assert.False(user.Active); // Users should be created as inactive
        }

        [Fact]
        public void TestActivateUser()
        {
            // Arrange
            var user = new User(
                new Email("test@example.com"),
                new Username("testuser"),
                new RoleId(Guid.NewGuid())
            );

            // Act
            user.Activate();

            // Assert
            Assert.True(user.Active);
        }

        [Fact]
        public void TestActivateAlreadyActiveUser_ShouldThrowException()
        {
            // Arrange
            var user = new User(
                new Email("test@example.com"),
                new Username("testuser"),
                new RoleId(Guid.NewGuid())
            );
            user.Activate(); // User is now active

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => user.Activate());
        }

        [Fact]
        public void TestDeactivateUser()
        {
            // Arrange
            var user = new User(
                new Email("test@example.com"),
                new Username("testuser"),
                new RoleId(Guid.NewGuid())
            );
            user.Activate(); // Must be active to be deactivated

            // Act
            user.Deactivate();

            // Assert
            Assert.False(user.Active);
        }

        [Fact]
        public void TestDeactivateAlreadyInactiveUser_ShouldThrowException()
        {
            // Arrange
            var user = new User(
                new Email("test@example.com"),
                new Username("testuser"),
                new RoleId(Guid.NewGuid())
            ); // User is inactive by default

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => user.Deactivate());
        }

        [Fact]
        public void TestChangeEmail_ActiveUser()
        {
            // Arrange
            var newEmail = new Email("new.email@example.com");
            var user = new User(
                new Email("test@example.com"),
                new Username("testuser"),
                new RoleId(Guid.NewGuid())
            );
            user.Activate();

            // Act
            user.ChangeEmail(newEmail);

            // Assert
            Assert.Equal(newEmail, user.Email);
        }

        [Fact]
        public void TestChangeEmail_InactiveUser_ShouldThrowException()
        {
            // Arrange
            var newEmail = new Email("new.email@example.com");
            var user = new User(
                new Email("test@example.com"),
                new Username("testuser"),
                new RoleId(Guid.NewGuid())
            ); // User is inactive

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(
                () => user.ChangeEmail(newEmail)
            );
        }

        [Fact]
        public void TestChangeUsername_ActiveUser()
        {
            // Arrange
            var newUsername = new Username("newTestUser");
            var user = new User(
                new Email("test@example.com"),
                new Username("testuser"),
                new RoleId(Guid.NewGuid())
            );
            user.Activate();

            // Act
            user.ChangeUsername(newUsername);

            // Assert
            Assert.Equal(newUsername, user.Username);
        }

        [Fact]
        public void TestChangeUsername_InactiveUser_ShouldThrowException()
        {
            // Arrange
            var newUsername = new Username("newTestUser");
            var user = new User(
                new Email("test@example.com"),
                new Username("testuser"),
                new RoleId(Guid.NewGuid())
            ); // User is inactive

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(
                () => user.ChangeUsername(newUsername)
            );
        }

        [Fact]
        public void TestChangeRoleId_ActiveUser()
        {
            // Arrange
            var newRoleId = new RoleId(Guid.NewGuid());
            var user = new User(
                new Email("test@example.com"),
                new Username("testuser"),
                new RoleId(Guid.NewGuid())
            );
            user.Activate();

            // Act
            user.ChangeRoleId(newRoleId);

            // Assert
            Assert.Equal(newRoleId, user.RoleId);
        }

        [Fact]
        public void TestChangeRoleId_InactiveUser_ShouldThrowException()
        {
            // Arrange
            var newRoleId = new RoleId(Guid.NewGuid());
            var user = new User(
                new Email("test@example.com"),
                new Username("testuser"),
                new RoleId(Guid.NewGuid())
            ); // User is inactive

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(
                () => user.ChangeRoleId(newRoleId)
            );
        }
    }
}