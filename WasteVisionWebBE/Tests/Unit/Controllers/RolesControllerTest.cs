using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using DDDSample1.Domain.Roles;
using DDDSample1.Domain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Controllers;
using System;
using DDDSample1.Application.Shared;

namespace DDDSample1.Tests.Controllers
{
    public class RolesControllerTest
    {
        private readonly Mock<IRoleService> _mockRoleService;
        private readonly RoleController _controller;

        private readonly Guid Id = Guid.NewGuid();

        public RolesControllerTest()
        {
            _mockRoleService = new Mock<IRoleService>();
            _controller = new RoleController(_mockRoleService.Object);
        }

        [Fact]
        public async Task GetAllRoles_ReturnsSuccess()
        {
            // Arrange
            var searchParams = new RoleSearchParamsDto();
            var roles = new List<RoleDto> { new RoleDto { Id = Id, Description = "Admin", Active = true } };
            _mockRoleService.Setup(s => s.GetAllWithFiltersAsync(searchParams))
                .ReturnsAsync(roles);

            // Act
            var result = await _controller.GetAllRoles(searchParams);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);

            // Verify the returned data matches our input roles
            Assert.Equal(roles, objectResult.Value);

            _mockRoleService.Verify(s => s.GetAllWithFiltersAsync(searchParams), Times.Once());
        }

        // Add these tests to the existing RolesControllerTest class

        [Fact]
        public async Task GetRole_WhenExists_ReturnsSuccess()
        {
            // Arrange
            var role = new RoleDto { Id = Id, Description = "Admin", Active = true };
            _mockRoleService.Setup(s => s.GetByIdAsync(It.IsAny<RoleId>()))
                .ReturnsAsync(role);

            // Act
            var result = await _controller.GetRole(Id.ToString());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.Equal(role, objectResult.Value);
        }

        [Fact]
        public async Task GetRole_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockRoleService.Setup(s => s.GetByIdAsync(It.IsAny<RoleId>()))
                .ReturnsAsync((RoleDto)null);

            // Act
            var result = await _controller.GetRole(Id.ToString());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.NotFoundError, objectResult.StatusCode);
        }

        [Fact]
        public async Task CreateRole_ReturnsSuccess()
        {
            // Arrange
            var createDto = new CreatingRoleDto("Admin");
            var createdRole = new RoleDto { Id = Id, Description = "Admin", Active = true };
            _mockRoleService.Setup(s => s.AddAsync(createDto))
                .ReturnsAsync(createdRole);

            // Act
            var result = await _controller.CreateRole(createDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.Equal(createdRole, objectResult.Value);
        }

        [Fact]
        public async Task UpdateRole_WhenExists_ReturnsSuccess()
        {
            // Arrange
            var updateDto = new RoleDto { Id = Id, Description = "Updated Admin", Active = true };
            _mockRoleService.Setup(s => s.UpdateAsync(Id.ToString(), updateDto))
                .ReturnsAsync(updateDto);

            // Act
            var result = await _controller.UpdateRole(Id.ToString(), updateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.Equal(updateDto, objectResult.Value);
        }

        [Fact]
        public async Task UpdateRole_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            var updateDto = new RoleDto { Id = Id, Description = "Updated Admin", Active = true };
            _mockRoleService.Setup(s => s.UpdateAsync(Id.ToString(), updateDto))
                .ReturnsAsync((RoleDto)null);

            // Act
            var result = await _controller.UpdateRole(Id.ToString(), updateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.NotFoundError, objectResult.StatusCode);
        }

        [Fact]
        public async Task DeactivateRole_WhenExists_ReturnsSuccess()
        {
            // Arrange
            _mockRoleService.Setup(s => s.DeactivateAsync(It.IsAny<RoleId>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeactivateRole(Id.ToString());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.True((bool)((dynamic)objectResult.Value));
        }

        [Fact]
        public async Task DeactivateRole_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockRoleService.Setup(s => s.DeactivateAsync(It.IsAny<RoleId>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeactivateRole(Id.ToString());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.NotFoundError, objectResult.StatusCode);
        }

        [Fact]
        public async Task ActivateRole_WhenExists_ReturnsSuccess()
        {
            // Arrange
            _mockRoleService.Setup(s => s.ActivateAsync(It.IsAny<RoleId>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ActivateRole(Id.ToString());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.True((bool)((dynamic)objectResult.Value));
        }

        [Fact]
        public async Task ActivateRole_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockRoleService.Setup(s => s.ActivateAsync(It.IsAny<RoleId>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.ActivateRole(Id.ToString());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.NotFoundError, objectResult.StatusCode);
        }

        [Fact]
        public async Task DeleteRole_WhenExists_ReturnsSuccess()
        {
            // Arrange
            _mockRoleService.Setup(s => s.DeleteAsync(It.IsAny<RoleId>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteRole(Id.ToString());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.True((bool)((dynamic)objectResult.Value));
        }

        [Fact]
        public async Task DeleteRole_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockRoleService.Setup(s => s.DeleteAsync(It.IsAny<RoleId>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteRole(Id.ToString());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.NotFoundError, objectResult.StatusCode);
        }

        [Fact]
        public async Task AllMethods_WhenExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            var exception = new BusinessRuleValidationException("Test error");
            _mockRoleService.Setup(s => s.GetAllWithFiltersAsync(It.IsAny<RoleSearchParamsDto>()))
                .ThrowsAsync(exception);

            // Act
            var result = await _controller.GetAllRoles(new RoleSearchParamsDto());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.BadRequestError, objectResult.StatusCode);
            Assert.Equal("Test error", ((dynamic)objectResult.Value).message);
        }
    }
}