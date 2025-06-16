using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DDDSample1.Domain.Detections
{
    public interface IDetectionService
    {

        Task<ImageAnalysisResultDto> UploadAndDetectAsync(IFormFile file, string modelId, string scheme, HostString host);
    }
}
