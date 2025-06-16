using System.Threading.Tasks;

namespace DDDSample1.Domain.RoboflowModels
{
    public interface IRoboflowModelInfoService
    {
        Task<RoboflowModelInfoDto> GetModelInfoAsync(string modelUrl, string apiKey);
    }
}