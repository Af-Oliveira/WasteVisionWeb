using System;
using Xunit;
using DDDSample1.Domain.Predictions;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.RoboflowModels;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Tests.Domain.Predictions
{
    public class PredictionTest
    {
        [Fact]
        public void TestPredictionCreation()
        {
            // Arrange
            var userId = new UserId(Guid.NewGuid());
            var modelId = new RoboflowModelId(Guid.NewGuid());
            var originalUrl = new Url("https://example.com/original.jpg");
            var processedUrl = new Url("https://example.com/processed.jpg");

            // Capture time just before creation to validate the auto-generated date
            var timeBeforeCreation = DateTime.UtcNow;

            // Act
            var prediction = new Prediction(userId, modelId, originalUrl, processedUrl);

            // Assert
            // 1. Verify that a new ID was generated
            Assert.NotNull(prediction.Id);

            // 2. Verify that all constructor parameters were assigned correctly
            Assert.Equal(userId, prediction.UserId);
            Assert.Equal(modelId, prediction.RoboflowModelId);
            Assert.Equal(originalUrl, prediction.OriginalImageUrl);
            Assert.Equal(processedUrl, prediction.ProcessedImageUrl);

            // 3. Verify that the Date was auto-initialized
            Assert.NotNull(prediction.Date);
            Assert.True(prediction.Date.Value >= timeBeforeCreation);
            Assert.True(prediction.Date.Value <= DateTime.UtcNow);

            // 4. Verify that the collection of ObjectPredictions is initialized and empty
            Assert.NotNull(prediction.ObjectPredictions);
            Assert.Empty(prediction.ObjectPredictions);
        }
    }
}