using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDDSample1.Domain.Detections
{
    public interface IImageProcessorService
    {
        /// <summary>
        /// Draws bounding boxes on an image based on prediction data.
        /// </summary>
        /// <param name="originalImageBytes">The byte array of the original image.</param>
        /// <param name="predictions">The list of predictions containing coordinates and classes.</param>
        /// <returns>A byte array representing the processed image with bounding boxes, or null if processing fails.</returns>
        Task<byte[]?> DrawBoundingBoxesAsync(byte[] originalImageBytes, List<RoboflowPredictionDTO> predictions);
    }
}