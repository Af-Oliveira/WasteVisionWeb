// filepath: c:\Users\afons\Documents\Work\PESTI\WasteVisionWeb\WasteVisionWebBE\Domain\Detection\DetectionResultDto.cs
namespace DDDSample1.Domain.Detections
{
    // Represents the combined result of uploading, detecting, and processing an image
    public class ImageAnalysisResultDto // Renamed from DetectionResultDto
    {
        public string OriginalImageUrl { get; set; }
        public RoboflowPredictionResponseDTO? Predictions { get; set; }
        public string? ProcessedImageUrl { get; set; } // URL of the image with boxes drawn

        // Constructor for convenience
        public ImageAnalysisResultDto(string originalImageUrl, RoboflowPredictionResponseDTO? predictions, string? processedImageUrl)
        {
            OriginalImageUrl = originalImageUrl;
            Predictions = predictions;
            ProcessedImageUrl = processedImageUrl;
        }
    }
}