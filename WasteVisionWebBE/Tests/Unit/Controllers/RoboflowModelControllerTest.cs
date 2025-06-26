using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Application.Shared;
using DDDSample1.Controllers;
using DDDSample1.Domain.RoboflowModels;
using DDDSample1.Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DDDSample1.Tests.Controllers
{
    public class RoboflowModelControllerTest
    {
        private readonly Mock<IRoboflowModelService> _mockService;
        private readonly RoboflowModelController _controller;
        private readonly Guid _testId = Guid.NewGuid();

        public RoboflowModelControllerTest()
        {
            _mockService = new Mock<IRoboflowModelService>();
            _controller = new RoboflowModelController(_mockService.Object);
        }

        [Fact]
        public async Task GetModel_WhenExists_ReturnsSuccess()
        {
            // Arrange
            var dto = new RoboflowModelDto(
                _testId,
                "Test Model",
                "key",
                "url",
                "path",
                "endpoint",
                "0.9",
                "0.8",
                "0.85",
                true
            );
            _mockService
                .Setup(s => s.GetByIdAsync(It.Is<RoboflowModelId>(id => id.Value == _testId.ToString())))
                .ReturnsAsync(dto);

            // Act
            var result = await _controller.GetModel(_testId.ToString());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.Equal(dto, ((dynamic)objectResult.Value));
        }

        [Fact]
        public async Task GetModel_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetByIdAsync(It.IsAny<RoboflowModelId>()))
                .ReturnsAsync((RoboflowModelDto)null);

            // Act
            var result = await _controller.GetModel(_testId.ToString());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.NotFoundError, objectResult.StatusCode);
        }

        [Fact]
        public async Task CreateModel_ReturnsSuccess()
        {
            // Arrange
            var createDto = new CreatingRoboflowModelDto
            {
                Description = "New Model",
                ApiKey = "new-key",
                ModelUrl = "new-url"
                // IFormFile is null in unit test, which is fine as the controller just passes it
            };
            var createdDto = new RoboflowModelDto(
                _testId,
                createDto.Description,
                createDto.ApiKey,
                createDto.ModelUrl,
                "some/path",
                "endpoint",
                "0.9",
                "0.8",
                "0.85",
                true
            );
            _mockService.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(createdDto);

            // Act
            var result = await _controller.CreateModel(createDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.Equal(createdDto, ((dynamic)objectResult.Value));
        }

        [Fact]
        public async Task UpdateModel_WhenExists_ReturnsSuccess()
        {
            // Arrange
            var updateDto = new UpdatingRoboflowModelDto
            {
                Description = "Updated Model"
            };
            var updatedDto = new RoboflowModelDto(
                _testId,
                updateDto.Description,
                "key",
                "url",
                "path",
                "endpoint",
                "0.9",
                "0.8",
                "0.85",
                true
            );
            _mockService
                .Setup(s => s.UpdateAsync(_testId.ToString(), updateDto))
                .ReturnsAsync(updatedDto);

            // Act
            var result = await _controller.UpdateModel(_testId.ToString(), updateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.Equal(updatedDto, ((dynamic)objectResult.Value));
        }

        [Fact]
        public async Task UpdateModel_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.UpdateAsync(It.IsAny<string>(), It.IsAny<UpdatingRoboflowModelDto>()))
                .ReturnsAsync((RoboflowModelDto)null);

            // Act
            var result = await _controller.UpdateModel(
                _testId.ToString(),
                new UpdatingRoboflowModelDto()
            );

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.NotFoundError, objectResult.StatusCode);
        }

        [Fact]
        public async Task DeleteModel_WhenExists_ReturnsSuccess()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteAsync(It.Is<RoboflowModelId>(id => id.Value == _testId.ToString())))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteModel(_testId.ToString());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.True(((dynamic)objectResult.Value));
        }

        [Fact]
        public async Task DeleteModel_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteAsync(It.IsAny<RoboflowModelId>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteModel(_testId.ToString());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.NotFoundError, objectResult.StatusCode);
        }

        [Fact]
        public async Task ActivateModel_WhenExists_ReturnsSuccess()
        {
            // Arrange
            _mockService
                .Setup(s => s.ActivateAsync(It.Is<RoboflowModelId>(id => id.Value == _testId.ToString())))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ActivateModel(_testId.ToString());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.Success, objectResult.StatusCode);
            Assert.True(((dynamic)objectResult.Value));
        }

        [Fact]
        public async Task DeactivateModel_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeactivateAsync(It.IsAny<RoboflowModelId>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeactivateModel(_testId.ToString());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.NotFoundError, objectResult.StatusCode);
        }

        [Fact]
        public async Task AnyMethod_WhenBusinessRuleExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            var exception = new BusinessRuleValidationException("Test error");
            _mockService.Setup(s => s.CreateAsync(It.IsAny<CreatingRoboflowModelDto>()))
                .ThrowsAsync(exception);

            // Act
            var result = await _controller.CreateModel(new CreatingRoboflowModelDto());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.BadRequestError, objectResult.StatusCode);
            Assert.Equal("Test error", ((dynamic)objectResult.Value).message);
        }

        [Fact]
        public async Task AnyMethod_WhenGeneralExceptionThrown_ReturnsServerError()
        {
            // Arrange
            var exception = new Exception("Unexpected error");
            _mockService.Setup(s => s.GetAllActiveAsync()).ThrowsAsync(exception);

            // Act
            var result = await _controller.GetAllActiveModels();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)StatusCodeEnum.ServerError, objectResult.StatusCode);
        }
    }
}