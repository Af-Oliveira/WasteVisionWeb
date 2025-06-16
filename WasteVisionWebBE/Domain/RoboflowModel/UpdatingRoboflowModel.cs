// filepath: DDDSample1/Domain/RoboflowModels/UpdatingRoboflowModelDto.cs
using Microsoft.AspNetCore.Http; // Required for IFormFile
using System.ComponentModel.DataAnnotations;

namespace DDDSample1.Domain.RoboflowModels
{
    public class UpdatingRoboflowModelDto
    {
        [Required(ErrorMessage = "Description is required")]
        [StringLength(100, ErrorMessage = "Description must be between 2 and 100 characters", MinimumLength = 2)]
        public string Description { get; set; }

        [Required(ErrorMessage = "API Key is required")]
        [StringLength(100, ErrorMessage = "API Key must be between 2 and 100 characters", MinimumLength = 2)]
        public string ApiKey { get; set; }

        [Required(ErrorMessage = "Model URL is required")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string ModelUrl { get; set; }

        // New optional property for uploading a replacement model file
        public IFormFile ModelFile { get; set; } 
    }
}
