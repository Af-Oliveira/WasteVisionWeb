using System;
using Xunit;
using DDDSample1.Domain.RoboflowModels;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Tests.Domain.RoboflowModels
{
    public class RoboflowModelTest
    {
        [Fact]
        public void TestRoboflowModelCreation()
        {
            // Arrange
            var apiKey = new ApiKey("test0api0key");
            var modelUrl = new Url("https://example.com/model");
            var localModelPath = new FilePath("/path/to/model");
            var description = new Description("Test Model");
            var endPoint = new Url("https://example.com/api");
            var map = new NumberDouble(0.85);
            var recall = new NumberDouble(0.8);
            var precision = new NumberDouble(0.9);

            // Act
            var model = new RoboflowModel(
                apiKey,
                modelUrl,
                localModelPath,
                description,
                endPoint,
                map,
                recall,
                precision
            );

            // Assert
            Assert.NotNull(model.Id);
            Assert.Equal(apiKey, model.ApiKey);
            Assert.Equal(modelUrl, model.ModelUrl);
            Assert.Equal(localModelPath, model.LocalModelPath);
            Assert.Equal(description, model.Description);
            Assert.Equal(endPoint, model.EndPoint);
            Assert.Equal(map, model.Map);
            Assert.Equal(recall, model.Recall);
            Assert.Equal(precision, model.Precision);
            Assert.True(model.Active);
        }

        [Fact]
        public void TestActivateModel()
        {
            // Arrange
            var model = CreateTestModel(false); // Create an inactive model

            // Act
            model.Activate();

            // Assert
            Assert.True(model.Active);
        }

        [Fact]
        public void TestActivateAlreadyActiveModel_ShouldThrowException()
        {
            // Arrange
            var model = CreateTestModel(true); // Create an active model

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => model.Activate());
        }

        [Fact]
        public void TestDeactivateModel()
        {
            // Arrange
            var model = CreateTestModel(true); // Create an active model

            // Act
            model.Deactivate();

            // Assert
            Assert.False(model.Active);
        }

        [Fact]
        public void TestDeactivateAlreadyInactiveModel_ShouldThrowException()
        {
            // Arrange
            var model = CreateTestModel(false); // Create an inactive model

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => model.Deactivate());
        }

        [Fact]
        public void TestChangeApiKey_ActiveModel()
        {
            // Arrange
            var model = CreateTestModel(true);
            var newApiKey = new ApiKey("new0api0key");

            // Act
            model.ChangeApiKey(newApiKey);

            // Assert
            Assert.Equal(newApiKey, model.ApiKey);
        }

        [Fact]
        public void TestChangeApiKey_InactiveModel_ShouldThrowException()
        {
            // Arrange
            var model = CreateTestModel(false);
            var newApiKey = new ApiKey("new0api0key");

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => model.ChangeApiKey(newApiKey));
        }

        [Fact]
        public void TestChangeModelUrl_ActiveModel()
        {
            // Arrange
            var model = CreateTestModel(true);
            var newModelUrl = new Url("https://new.example.com/model");

            // Act
            model.ChangeModelUrl(newModelUrl);

            // Assert
            Assert.Equal(newModelUrl, model.ModelUrl);
        }

        [Fact]
        public void TestChangeModelUrl_InactiveModel_ShouldThrowException()
        {
            // Arrange
            var model = CreateTestModel(false);
            var newModelUrl = new Url("https://new.example.com/model");

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => model.ChangeModelUrl(newModelUrl));
        }

        [Fact]
        public void TestChangeDescription_ActiveModel()
        {
            // Arrange
            var model = CreateTestModel(true);
            var newDescription = new Description("New Description");

            // Act
            model.ChangeDescription(newDescription);

            // Assert
            Assert.Equal(newDescription, model.Description);
        }

        [Fact]
        public void TestChangeDescription_InactiveModel_ShouldThrowException()
        {
            // Arrange
            var model = CreateTestModel(false);
            var newDescription = new Description("New Description");

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => model.ChangeDescription(newDescription));
        }

        [Fact]
        public void TestChangeEndPoint_ActiveModel()
        {
            // Arrange
            var model = CreateTestModel(true);
            var newEndPoint = new Url("https://new.example.com/api");

            // Act
            model.ChangeEndPoint(newEndPoint);

            // Assert
            Assert.Equal(newEndPoint, model.EndPoint);
        }

        [Fact]
        public void TestChangeEndPoint_InactiveModel_ShouldThrowException()
        {
            // Arrange
            var model = CreateTestModel(false);
            var newEndPoint = new Url("https://new.example.com/api");

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => model.ChangeEndPoint(newEndPoint));
        }

        [Fact]
        public void TestChangeMap_ActiveModel()
        {
            // Arrange
            var model = CreateTestModel(true);
            var newMap = new NumberDouble(0.9);

            // Act
            model.ChangeMap(newMap);

            // Assert
            Assert.Equal(newMap, model.Map);
        }

        [Fact]
        public void TestChangeMap_InactiveModel_ShouldThrowException()
        {
            // Arrange
            var model = CreateTestModel(false);
            var newMap = new NumberDouble(0.9);

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => model.ChangeMap(newMap));
        }

        [Fact]
        public void TestChangeRecall_ActiveModel()
        {
            // Arrange
            var model = CreateTestModel(true);
            var newRecall = new NumberDouble(0.9);

            // Act
            model.ChangeRecall(newRecall);

            // Assert
            Assert.Equal(newRecall, model.Recall);
        }

        [Fact]
        public void TestChangeRecall_InactiveModel_ShouldThrowException()
        {
            // Arrange
            var model = CreateTestModel(false);
            var newRecall = new NumberDouble(0.9);

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => model.ChangeRecall(newRecall));
        }

        [Fact]
        public void TestChangePrecision_ActiveModel()
        {
            // Arrange
            var model = CreateTestModel(true);
            var newPrecision = new NumberDouble(0.9);

            // Act
            model.ChangePrecision(newPrecision);

            // Assert
            Assert.Equal(newPrecision, model.Precision);
        }

        [Fact]
        public void TestChangePrecision_InactiveModel_ShouldThrowException()
        {
            // Arrange
            var model = CreateTestModel(false);
            var newPrecision = new NumberDouble(0.9);

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => model.ChangePrecision(newPrecision));
        }

        [Fact]
        public void TestChangeLocalModelUrl_ActiveModel()
        {
            // Arrange
            var model = CreateTestModel(true);
            var newLocalModelPath = new FilePath("/new/path/to/model");

            // Act
            model.ChangeLocalModelUrl(newLocalModelPath);

            // Assert
            Assert.Equal(newLocalModelPath, model.LocalModelPath);
        }

        [Fact]
        public void TestChangeLocalModelUrl_InactiveModel_ShouldThrowException()
        {
            // Arrange
            var model = CreateTestModel(false);
            var newLocalModelPath = new FilePath("/new/path/to/model");

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => model.ChangeLocalModelUrl(newLocalModelPath));
        }

        private RoboflowModel CreateTestModel(bool isActive)
        {
            var apiKey = new ApiKey("test0api0key");
            var modelUrl = new Url("https://example.com/model");
            var localModelPath = new FilePath("/path/to/model");
            var description = new Description("Test Model");
            var endPoint = new Url("https://example.com/api");
            var map = new NumberDouble(0.85);
            var recall = new NumberDouble(0.8);
            var precision = new NumberDouble(0.9);

            var model = new RoboflowModel(
                apiKey,
                modelUrl,
                localModelPath,
                description,
                endPoint,
                map,
                recall,
                precision
            );

            if (!isActive)
            {
                model.Deactivate();
            }

            return model;
        }
    }
}