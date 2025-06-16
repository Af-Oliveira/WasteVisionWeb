using System;
using Xunit;
using DDDSample1.Domain.Roles;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Tests.Domain.Roles
{
    public class RoleTest
    {
        [Fact]
        public void TestRoleCreation()
        {
            // Arrange
            var description = new Description("Admin");

            // Act
            var role = new Role(description);

            // Assert
            Assert.NotNull(role.Id);
            Assert.Equal(description, role.Description);
            Assert.True(role.Active);
        }

        [Fact]
        public void TestChangeDescription()
        {
            // Arrange
            var description = new Description("Admin");
            var newDescription = new Description("User");
            var role = new Role(description);

            // Act
            role.ChangeDescription(newDescription);

            // Assert
            Assert.Equal(newDescription, role.Description);
        }

        [Fact]
        public void TestChangeDescriptionInactiveRole()
        {
            // Arrange
            var description = new Description("Admin");
            var newDescription = new Description("User");
            var role = new Role(description);
            role.Deactivate();

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => role.ChangeDescription(newDescription));
        }

        [Fact]
        public void TestDeactivateRole()
        {
            // Arrange
            var description = new Description("Admin");
            var role = new Role(description);

            // Act
            role.Deactivate();

            // Assert
            Assert.False(role.Active);
        }

        [Fact]
        public void TestActivateRole()
        {
            // Arrange
            var description = new Description("Admin");
            var role = new Role(description);
            role.Deactivate();

            // Act
            role.Activate();

            // Assert
            Assert.True(role.Active);
        }
    }
}