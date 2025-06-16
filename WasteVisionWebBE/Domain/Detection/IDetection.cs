using System.Threading.Tasks;

namespace DDDSample1.Domain.Detections
{
    public interface IDetection
    {
        Task<RoboflowPredictionResponseDTO?> DetectObjectsAsync(byte[] imageBytes, string localModel, string modelUrl, string apiKey);
    }
}