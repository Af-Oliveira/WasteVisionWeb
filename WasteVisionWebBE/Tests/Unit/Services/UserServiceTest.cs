using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.Roles;
using Moq;
using Xunit;

namespace DDDSample1.Tests.Services
{
    public class UserServiceTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly IUserService _service;

        // Test Data
        private readonly UserId _testId = new UserId(Guid.NewGuid());
        private readonly Email _testEmail = new Email("test@example.com");
        private readonly Username _testUsername = new Username("testuser");
        private readonly RoleId _testRoleId = new RoleId(Guid.NewGuid());
        private readonly Role _testRole; // The associated Role object

        public UserServiceTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IUserRepository>();
            _service = new UserService(_mockUnitOfWork.Object, _mockRepo.Object);

            // Initialize the Role object for our tests
            _testRole = new Role(new Description("Test Role"));
        }

        // Helper method to create a realistic User object for testing
        private User CreateTestUser(bool isActive = true)
        {
            var user = new User(_testEmail, _testUsername, _testRoleId);
            if (!isActive)
            {
                user.Deactivate();
            }

            // Use reflection to set the private Role property, mimicking EF Core's .Include()
            var roleProperty =
                typeof(User).GetProperty("Role", BindingFlags.Public | BindingFlags.Instance);
            if (roleProperty != null && roleProperty.CanWrite)
            {
                roleProperty.SetValue(user, _testRole, null);
            }
            else
            {
                // Fallback for if the property setter is not accessible (e.g., truly private)
                var backingField = typeof(User).GetField(
                    "<Role>k__BackingField",
                    BindingFlags.Instance | BindingFlags.NonPublic
                );
                backingField?.SetValue(user, _testRole);
            }

            return user;
        }

        [Fact]
        public async Task GetAllWithFiltersAsync_ReturnsUsers()
        {
            // Arrange
            var searchParams = new UserSearchParamsDto();
            var users = new List<User> { CreateTestUser() };
            _mockRepo
                .Setup(repo => repo.GetAllWithFiltersAsync(searchParams))
                .ReturnsAsync(users);

            // Act
            var result = await _service.GetAllWithFiltersAsync(searchParams);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            _mockRepo.Verify(
                repo => repo.GetAllWithFiltersAsync(searchParams),
                Times.Once()
            );
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsUser()
        {
            // Arrange
            var user = CreateTestUser();
            _mockRepo.Setup(repo => repo.GetByIdAsync(_testId)).ReturnsAsync(user);

            // Act
            var result = await _service.GetByIdAsync(_testId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_testRole.Description.Value, result.RoleName); // Verify role data is present
            _mockRepo.Verify(repo => repo.GetByIdAsync(_testId), Times.Once());
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<UserId>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _service.GetByIdAsync(_testId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_Success()
        {
            // Arrange
            var dto = new CreatingUserDto(
                _testEmail.Value,
                _testUsername.Value,
                _testRoleId.Value
            );
            var user = CreateTestUser();

            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<User>()));
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).ReturnsAsync(1);
            // The service calls GetByIdAsync after creating to return the full DTO.
            // Ensure this mock returns the user with the Role property populated.
            _mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<UserId>())).ReturnsAsync(user);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.RoleId); // Crucial check
            _mockRepo.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once());
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_WhenExists_ReturnsUpdatedUser()
        {
            // Arrange
            var user = CreateTestUser(true); // User must be active to be updated
            var dto = new UpdatingUserDto(
                "new@example.com",
                "newUsername",
                Guid.NewGuid().ToString()
            );
            _mockRepo.Setup(repo => repo.GetByIdAsync(new UserId(_testId.Value)))
                .ReturnsAsync(user);

            // Act
            var result = await _service.UpdateAsync(_testId.Value, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Email, result.Email);
            Assert.Equal(dto.Username, result.Username);

            Assert.Equal(dto.RoleId, result.RoleId);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once());
        }

        // ... other tests (Activate, Deactivate, Delete) remain the same
        // as they primarily test state changes and don't depend on the Role object itself.
        // However, their setup now benefits from the more realistic CreateTestUser helper.

        [Fact]
        public async Task ActivateUserAsync_WhenExists_ReturnsTrue()
        {
            // Arrange
            var user = CreateTestUser(false); // User starts inactive
            _mockRepo.Setup(repo => repo.GetByIdAsync(new UserId(_testId.Value)))
                .ReturnsAsync(user);

            // Act
            var result = await _service.ActivateUserAsync(_testId.Value);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once());
        }

        [Fact]
        public async Task DeactivateUserAsync_WhenExists_ReturnsTrue()
        {
            // Arrange
            var user = CreateTestUser(true); // User starts active
            _mockRepo.Setup(repo => repo.GetByIdAsync(new UserId(_testId.Value)))
                .ReturnsAsync(user);

            // Act
            var result = await _service.DeactivateUserAsync(_testId.Value);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_WhenExists_ReturnsTrue()
        {
            // Arrange
            var user = CreateTestUser();
            _mockRepo.Setup(repo => repo.GetByIdAsync(_testId)).ReturnsAsync(user);

            // Act
            var result = await _service.DeleteAsync(_testId);

            // Assert
            Assert.True(result);
            _mockRepo.Verify(repo => repo.Remove(user), Times.Once());
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once());
        }
    }
}