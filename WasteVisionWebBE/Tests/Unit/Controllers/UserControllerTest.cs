using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Controllers;
using DDDSample1.Application.Shared;

namespace DDDSample1.Tests.Controllers
{
    public class UserControllerTest
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _controller;
        private readonly string testId = "c9bc2716-c9f3-44e9-b8a1-c68e81d69912";

        public UserControllerTest()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UserController(_mockUserService.Object);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsSuccess()
        {
            // Arrange
            var searchParams = new UserSearchParamsDto();
            var users = new List<UserApiDto> { new UserApiDto() };
            _mockUserService.Setup(s => s.GetAllWithFiltersAsync(searchParams))
                .ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers(searchParams);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.Equal(users, ((dynamic)objectResult.Value));
        }

        [Fact]
        public async Task CreateUser_ReturnsSuccess()
        {
            // Arrange
            var createDto = new CreatingUserDto();
            var createdUser = new UserApiDto();
            _mockUserService.Setup(s => s.CreateAsync(createDto))
                .ReturnsAsync(createdUser);

            // Act
            var result = await _controller.CreateUser(createDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.Equal(createdUser, ((dynamic)objectResult.Value));
        }

        [Fact]
        public async Task GetUser_WhenExists_ReturnsSuccess()
        {
            // Arrange
            var user = new UserApiDto();
            _mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<UserId>()))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUser(testId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.Equal(user, ((dynamic)objectResult.Value));
        }

        [Fact]
        public async Task GetUserByEmail_WhenExists_ReturnsSuccess()
        {
            // Arrange
            var email = "test@example.com";
            var user = new UserApiDto();
            _mockUserService.Setup(s => s.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserByEmail(email);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.Equal(user, ((dynamic)objectResult.Value));
        }

        [Fact]
        public async Task UpdateUser_WhenExists_ReturnsSuccess()
        {
            // Arrange
            var updateDto = new UpdatingUserDto();
            var updatedUser = new UserApiDto();
            _mockUserService.Setup(s => s.UpdateAsync(testId, updateDto))
                .ReturnsAsync(updatedUser);

            // Act
            var result = await _controller.UpdateUser(testId, updateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.Equal(updatedUser, ((dynamic)objectResult.Value));
        }

        [Fact]
        public async Task DeactivateUser_WhenExists_ReturnsSuccess()
        {
            // Arrange
            _mockUserService.Setup(s => s.DeactivateUserAsync(testId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeactivateUser(testId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.True(((dynamic)objectResult.Value));
        }

        [Fact]
        public async Task ActivateUser_WhenExists_ReturnsSuccess()
        {
            // Arrange
            _mockUserService.Setup(s => s.ActivateUserAsync(testId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ActivateUser(testId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.True(((dynamic)objectResult.Value));
        }

        [Fact]
        public async Task DeleteUser_WhenExists_ReturnsSuccess()
        {
            // Arrange
            _mockUserService.Setup(s => s.DeleteAsync(It.IsAny<UserId>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteUser(testId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.True(((dynamic)objectResult.Value));
        }

        [Fact]
        public async Task AllMethods_WhenExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            var exception = new BusinessRuleValidationException("Test error");
            _mockUserService.Setup(s => s.GetAllWithFiltersAsync(It.IsAny<UserSearchParamsDto>()))
                .ThrowsAsync(exception);

            // Act
            var result = await _controller.GetAllUsers(new UserSearchParamsDto());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.BadRequestError, objectResult.StatusCode);
            Assert.Equal("Test error", ((dynamic)objectResult.Value).message);
        }
    }
}