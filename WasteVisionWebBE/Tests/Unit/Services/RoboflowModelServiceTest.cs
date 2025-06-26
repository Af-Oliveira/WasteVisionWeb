using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.RoboflowModels;
using DDDSample1.Domain.Logging;
using DDDSample1.Domain.Roles; // For IExceptionHandler
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace DDDSample1.Tests.Services
{
    public class RoboflowModelServiceTest
    {
        // Mocks for all dependencies
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRoboflowModelRepository> _mockRepo;
        private readonly Mock<ILogManager> _mockLogManager;
        private readonly Mock<IRoboflowModelInfoService> _mockModelInfoService;
        private readonly Mock<IExceptionHandler> _mockExceptionHandler;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;

        // The service under test
        private readonly RoboflowModelService _service;

        // Common test data
        private readonly RoboflowModelId _testId = new RoboflowModelId(Guid.NewGuid());
        private readonly string _testModelUrl = "https://serverless.roboflow.com/garbage-classification-3/2";
        private readonly string _testApiKey = "kKrV2TlFATeJEMDXdKLM";

        public RoboflowModelServiceTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IRoboflowModelRepository>();
            _mockLogManager = new Mock<ILogManager>();
            _mockModelInfoService = new Mock<IRoboflowModelInfoService>();
            _mockExceptionHandler = new Mock<IExceptionHandler>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();

            // Setup IWebHostEnvironment mock to return a dummy path
            _mockWebHostEnvironment
                .Setup(w => w.ContentRootPath)
                .Returns("C:\\fake_path");

            _service = new RoboflowModelService(
                _mockUnitOfWork.Object,
                _mockRepo.Object,
                _mockLogManager.Object,
                _mockModelInfoService.Object,
                _mockExceptionHandler.Object,
                _mockWebHostEnvironment.Object
            );
        }

        // Helper to create a standard RoboflowModel for tests
        private RoboflowModel CreateTestModel(bool isActive = true)
        {
            var model = new RoboflowModel(
                new ApiKey(_testApiKey),
                new Url(_testModelUrl),
                new FilePath("C:\\fake_path\\PythonBE\\models\\somefile.pt"),
                new Description("Test Model"),
                new Url("https://endpoint.com"),
                new NumberDouble(0.9),
                new NumberDouble(0.8),
                new NumberDouble(0.85)
            );
            if (!isActive)
            {
                model.Deactivate();
            }
            return model;
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsDto()
        {
            // Arrange
            var model = CreateTestModel();
            _mockRepo.Setup(repo => repo.GetByIdAsync(_testId)).ReturnsAsync(model);

            // Act
            var result = await _service.GetByIdAsync(_testId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(model.Description.Value, result.Description);
        }

        [Fact]
        public async Task CreateAsync_WithFile_Success()
        {
            // Arrange
            // 1. Mock the IFormFile
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.pt");
            mockFile.Setup(f => f.Length).Returns(1024);
            // This setup is crucial for the CopyToAsync method
            mockFile
                .Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<System.Threading.CancellationToken>()))
                .Returns(Task.CompletedTask);

            // 2. Create the DTO
            var dto = new CreatingRoboflowModelDto
            {
                Description = "Test Model",
                ApiKey = _testApiKey,
                ModelUrl = _testModelUrl,
                ModelFile = mockFile.Object
            };

            // 3. Mock the external Roboflow service call
            var modelInfo = new RoboflowModelInfoDto
            {
                Version = new VersionInfo
                {
                    Model = new ModelInfo
                    {
                        Endpoint = "https://endpoint.com",
                        Map = "0.95",
                        Recall = "0.92",
                        Precision = "0.98"
                    }
                }
            };
            _mockModelInfoService
                .Setup(s => s.GetModelInfoAsync(dto.ModelUrl, dto.ApiKey))
                .ReturnsAsync(modelInfo);

            // 4. Mock repository and unit of work
            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<RoboflowModel>()));
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).ReturnsAsync(1);
            _mockRepo
                .Setup(repo => repo.GetByIdAsync(It.IsAny<RoboflowModelId>()))
                .ReturnsAsync((RoboflowModelId id) => CreateTestModel()); // Return a valid model for the final step

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Description, result.Description);
            Assert.Equal(modelInfo.Version.Model.Endpoint, result.EndPoint); // Check if Roboflow data was mapped
            _mockModelInfoService.Verify(s => s.GetModelInfoAsync(dto.ModelUrl, dto.ApiKey), Times.Once);
            _mockRepo.Verify(repo => repo.AddAsync(It.IsAny<RoboflowModel>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenRoboflowValidationFails_ThrowsException()
        {
            // Arrange
            var dto = new CreatingRoboflowModelDto
            {
                Description = "New Model",
                ApiKey = "invalid-key",
                ModelUrl = _testModelUrl
            };
            var exception = new BusinessRuleValidationException("Invalid API key or model URL");
            _mockModelInfoService
                .Setup(s => s.GetModelInfoAsync(dto.ModelUrl, dto.ApiKey))
                .ThrowsAsync(exception);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _service.CreateAsync(dto));
            _mockRepo.Verify(repo => repo.AddAsync(It.IsAny<RoboflowModel>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenExistsAndChangesMade_ReturnsUpdatedDto()
        {
            // Arrange
            var model = CreateTestModel();
            var dto = new UpdatingRoboflowModelDto
            {
                Description = "Updated Description"
            };
            _mockRepo.Setup(repo => repo.GetByIdAsync(new RoboflowModelId(_testId.Value)))
                .ReturnsAsync(model);

            // Act
            var result = await _service.UpdateAsync(_testId.Value, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Description, result.Description);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenModelUrlChanges_FetchesNewRoboflowInfo()
        {
            // Arrange
            var model = CreateTestModel();
            var newModelUrl = "https://api.roboflow.com/v1/model/new-model";
            var dto = new UpdatingRoboflowModelDto { ModelUrl = newModelUrl };

            var newModelInfo = new RoboflowModelInfoDto
            {
                Version = new VersionInfo { Model = new ModelInfo { Endpoint = "https://new-endpoint.com", Map = "1.0", Recall = "1.0", Precision = "1.0" } }
            };

            _mockRepo.Setup(repo => repo.GetByIdAsync(new RoboflowModelId(_testId.Value)))
                .ReturnsAsync(model);
            _mockModelInfoService
                .Setup(s => s.GetModelInfoAsync(newModelUrl, model.ApiKey.Value))
                .ReturnsAsync(newModelInfo);

            // Act
            var result = await _service.UpdateAsync(_testId.Value, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newModelInfo.Version.Model.Endpoint, result.EndPoint);
            _mockModelInfoService.Verify(s => s.GetModelInfoAsync(newModelUrl, model.ApiKey.Value), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenExists_ReturnsTrue()
        {
            // Arrange
            var model = CreateTestModel();
            _mockRepo.Setup(repo => repo.GetByIdAsync(_testId)).ReturnsAsync(model);

            // Act
            var result = await _service.DeleteAsync(_testId);

            // Assert
            Assert.True(result);
            _mockRepo.Verify(repo => repo.Remove(model), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task ActivateAsync_WhenExistsAndInactive_ReturnsTrue()
        {
            // Arrange
            var model = CreateTestModel(isActive: false);
            _mockRepo.Setup(repo => repo.GetByIdAsync(_testId)).ReturnsAsync(model);

            // Act
            var result = await _service.ActivateAsync(_testId);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeactivateAsync_WhenExistsAndActive_ReturnsTrue()
        {
            // Arrange
            var model = CreateTestModel(isActive: true);
            _mockRepo.Setup(repo => repo.GetByIdAsync(_testId)).ReturnsAsync(model);

            // Act
            var result = await _service.DeactivateAsync(_testId);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }
    }
}