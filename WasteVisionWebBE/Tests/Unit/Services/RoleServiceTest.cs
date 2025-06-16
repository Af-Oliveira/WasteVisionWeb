using Moq;
using Xunit;
using DDDSample1.Domain.Roles;
using DDDSample1.Domain.Shared;
using System.Threading.Tasks;
using System.Collections.Generic;
using DDDSample1.Domain.Logging;

namespace DDDSample1.Tests.Services
{
    public class RoleServiceTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRoleRepository> _mockRepo;
        private readonly Mock<ILogManager> _mockLogManager;
        private readonly RoleService _service;
        private readonly RoleId testId = new RoleId("7df96d65-e543-4854-b655-a979c348506e");

        public RoleServiceTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IRoleRepository>();
            _mockLogManager = new Mock<ILogManager>();
            _service = new RoleService(_mockUnitOfWork.Object, _mockRepo.Object, _mockLogManager.Object);
        }

        [Fact]
        public async Task GetAllWithFiltersAsync_ReturnsRoles()
        {
            // Arrange
            var searchParams = new RoleSearchParamsDto();
            var roles = new List<Role> { new Role(new Description("Admin")) };
            _mockRepo.Setup(repo => repo.GetAllWithFiltersAsync(searchParams))
                .ReturnsAsync(roles);

            // Act
            var result = await _service.GetAllWithFiltersAsync(searchParams);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            _mockRepo.Verify(repo => repo.GetAllWithFiltersAsync(searchParams), Times.Once());
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsRole()
        {
            // Arrange
            var role = new Role(new Description("Admin"));
            _mockRepo.Setup(repo => repo.GetByIdAsync(testId))
                .ReturnsAsync(role);

            // Act
            var result = await _service.GetByIdAsync(testId);

            // Assert
            Assert.NotNull(result);
            _mockRepo.Verify(repo => repo.GetByIdAsync(testId), Times.Once());
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetByIdAsync(testId))
                .ReturnsAsync((Role)null);

            // Act
            var result = await _service.GetByIdAsync(testId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_Success()
        {
            // Arrange
            var dto = new CreatingRoleDto("Admin");
            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Role>()))
                .ReturnsAsync((Role r) => r);
            _mockUnitOfWork.Setup(uow => uow.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.NotNull(result);
            _mockRepo.Verify(repo => repo.AddAsync(It.IsAny<Role>()), Times.Once());
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once());
            _mockLogManager.Verify(log => log.Write(LogType.Role, It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_WhenExists_ReturnsUpdatedRole()
        {
            // Arrange
            var role = new Role(new Description("Admin"));
            var dto = new RoleDto { Description = "Updated Admin" };
            _mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<RoleId>()))
                .ReturnsAsync(role);

            // Act
            var result = await _service.UpdateAsync(testId.AsString(), dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Description, result.Description);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once());
        }

        [Fact]
        public async Task DeactivateAsync_WhenExists_ReturnsTrue()
        {
            // Arrange
            var role = new Role(new Description("Admin"));
            _mockRepo.Setup(repo => repo.GetByIdAsync(testId))
                .ReturnsAsync(role);

            // Act
            var result = await _service.DeactivateAsync(testId);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once());
        }

        [Fact]
        public async Task ActivateAsync_WhenExists_ReturnsTrue()
        {
            // Arrange
            var role = new Role(new Description("Admin"));
            role.Deactivate();
            _mockRepo.Setup(repo => repo.GetByIdAsync(testId))
                .ReturnsAsync(role);

            // Act
            var result = await _service.ActivateAsync(testId);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_WhenExistsAndInactive_ReturnsTrue()
        {
            // Arrange
            var role = new Role(new Description("Admin"));
            role.Deactivate();
            _mockRepo.Setup(repo => repo.GetByIdAsync(testId))
                .ReturnsAsync(role);

            // Act
            var result = await _service.DeleteAsync(testId);

            // Assert
            Assert.True(result);
            _mockRepo.Verify(repo => repo.Remove(role), Times.Once());
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_WhenActive_ThrowsException()
        {
            // Arrange
            var role = new Role(new Description("Admin"));
            _mockRepo.Setup(repo => repo.GetByIdAsync(testId))
                .ReturnsAsync(role);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(
                () => _service.DeleteAsync(testId));
        }

        [Fact]
        public async Task GetRoleByDescriptionAsync_WhenExists_ReturnsRole()
        {
            // Arrange
            var description = new Description("Admin");
            var role = new Role(description);
            _mockRepo.Setup(repo => repo.GetRoleByDescriptionAsync(description))
                .ReturnsAsync(role);

            // Act
            var result = await _service.GetRoleByDescriptionAsync(description);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(description.AsString(), result.Description);
        }
    }
}