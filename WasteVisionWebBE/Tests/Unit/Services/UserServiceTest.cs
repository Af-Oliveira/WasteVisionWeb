using System;
using System.Collections.Generic;
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
        private readonly UserId _testId = new UserId(Guid.NewGuid());
        private readonly Email _testEmail = new Email("test@example.com");
        private readonly Username _testUsername = new Username("testuser");
        private readonly RoleId _testRoleId = new RoleId(Guid.NewGuid());

        public UserServiceTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IUserRepository>();
            _service = new UserService(_mockUnitOfWork.Object, _mockRepo.Object);
        }

        private User CreateTestUser(bool isActive = false)
        {
            var user = new User(_testEmail, _testUsername, _testRoleId);
            if (isActive)
            {
                user.Activate();
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
            // The service calls GetByIdAsync after creating to return the full DTO
            _mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<UserId>())).ReturnsAsync(user);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
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

        [Fact]
        public async Task DeleteAsync_WhenNotFound_ReturnsFalse()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetByIdAsync(_testId)).ReturnsAsync((User)null);

            // Act
            var result = await _service.DeleteAsync(_testId);

            // Assert
            Assert.False(result);
            _mockRepo.Verify(repo => repo.Remove(It.IsAny<User>()), Times.Never());
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Never());
        }
    }
}