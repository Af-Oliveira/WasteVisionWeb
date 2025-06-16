using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.PixelFormats; // For Rgba32
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace DDDSample1.Domain.Detections
{
    public class ImageProcessorService : IImageProcessorService
    {
        private readonly ILogger<ImageProcessorService> _logger;
        private readonly Font _font; // Font for labels

        // Define colors for different classes (add more as needed)
        private static readonly Dictionary<string, Color> ClassColors = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase)
        {
            { "METAL", Color.Red },
            { "PLASTIC", Color.Blue },
            { "GLASS", Color.Green },
            { "PAPER", Color.Yellow },
            { "CARDBOARD", Color.Orange },
            { "TRASH", Color.Gray }
        };
        private static readonly Color DefaultColor = Color.Magenta; // Color for unknown classes
        private const float BoxThickness = 2f; // Thickness of the bounding box lines

        // Define minimum dimensions for readability
        private const int MinImageDimension = 1000;

        public ImageProcessorService(ILogger<ImageProcessorService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Font Loading - Using system fonts for reliability
            try
            {
             // Try to find a common system font that should be available
                string[] commonFonts = { "Arial", "Verdana", "Calibri", "Segoe UI", "Tahoma" };
                
                FontFamily fontFamily;
                foreach (var fontName in commonFonts)
                {
                    fontFamily = SystemFonts.Families.FirstOrDefault(f => 
                        f.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase));
                    
                    if (fontFamily != null)
                    {
                        _logger.LogInformation("Found system font: {FontName}", fontFamily.Name);
                        break;
                    }
                }

                if (fontFamily != null)
                {
                    _font = fontFamily.CreateFont(12, FontStyle.Regular);
                    _logger.LogInformation("Successfully loaded font for image processing: {FontName}", fontFamily.Name);
                }
                else if (SystemFonts.Families.Any())
                {
                    // Fallback to the first available system font
                    fontFamily = SystemFonts.Families.First();
                    _font = fontFamily.CreateFont(12, FontStyle.Regular);
                    _logger.LogInformation("Using fallback system font: {FontName}", fontFamily.Name);
                }
                else
                {
                    _logger.LogWarning("No system fonts available. Text labels will not be drawn.");
                    _font = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load any font for image processing. Labels will not be drawn.");
                _font = null;
            }
        }

        public async Task<byte[]?> DrawBoundingBoxesAsync(byte[] originalImageBytes, List<RoboflowPredictionDTO> predictions)
        {
            if (originalImageBytes == null || originalImageBytes.Length == 0)
            {
                _logger.LogWarning("DrawBoundingBoxesAsync called with null or empty image bytes.");
                return null;
            }
            if (predictions == null || predictions.Count == 0)
            {
                _logger.LogInformation("DrawBoundingBoxesAsync called with no predictions. Returning original image bytes.");
                return originalImageBytes; // No boxes to draw
            }

            try
            {
                using (Image image = Image.Load(originalImageBytes))
                {
                    // Check if image needs upscaling for better label readability
                    bool wasUpscaled = false;
                    float upscaleFactor = 1.0f;
                    int originalWidth = image.Width;
                    int originalHeight = image.Height;

                    if (image.Width < MinImageDimension || image.Height < MinImageDimension)
                    {
                        // Calculate scale factor to make the smaller dimension reach 1000px
                        float widthScale = (float)MinImageDimension / image.Width;
                        float heightScale = (float)MinImageDimension / image.Height;
                        upscaleFactor = Math.Min(widthScale, heightScale); // Take the smaller scale to ensure at least one dimension reaches 1000px

                        _logger.LogInformation("Upscaling image from {OriginalWidth}x{OriginalHeight} by factor {Factor}", 
                            image.Width, image.Height, upscaleFactor);

                        // Resize the image while maintaining aspect ratio
                        image.Mutate(ctx => ctx.Resize(
                            (int)(image.Width * upscaleFactor), 
                            (int)(image.Height * upscaleFactor),
                            KnownResamplers.Lanczos3)); // Lanczos3 is good for upscaling

                        wasUpscaled = true;
                        _logger.LogInformation("Image upscaled to {NewWidth}x{NewHeight}", image.Width, image.Height);
                    }

                    foreach (var prediction in predictions)
                    {
                        // If image was upscaled, we need to scale the coordinates too
                        float x = (float)prediction.X;
                        float y = (float)prediction.Y;
                        float width = (float)prediction.Width;
                        float height = (float)prediction.Height;

                        if (wasUpscaled)
                        {
                            x *= upscaleFactor;
                            y *= upscaleFactor;
                            width *= upscaleFactor;
                            height *= upscaleFactor;
                        }

                        // Roboflow provides center x, y. Convert to top-left for drawing.
                        float xMin = x - width / 2;
                        float yMin = y - height / 2;

                        // Ensure coordinates are within image bounds
                        xMin = Math.Max(0, xMin);
                        yMin = Math.Max(0, yMin);
                        width = Math.Min(image.Width - xMin, width);
                        height = Math.Min(image.Height - yMin, height);

                        if (width <= 0 || height <= 0) continue; // Skip invalid boxes

                        var rect = new RectangleF(xMin, yMin, width, height);

                        // Choose color based on class
                        Color boxColor = ClassColors.TryGetValue(prediction.Class, out var color) ? color : DefaultColor;

                        // Draw the bounding box with appropriate thickness
                        var pen = Pens.Solid(boxColor, wasUpscaled ? BoxThickness * 2 : BoxThickness);
                        image.Mutate(ctx => ctx.Draw(pen, rect));

                        // Draw label if font is available
                        if (_font != null)
                        {
                            string label = $"{prediction.Class} ({(prediction.Confidence * 100):0.0}%)";
                            
                            // Ensure label position is within image bounds
                            float labelY = yMin - 25; // Position above box with more space
                            if (labelY < 0) labelY = yMin + 5; // Move inside box if too high
                            
                            // Calculate a font size based on the image scale
                            float fontSizeAdjustment = wasUpscaled ? 1.5f : 1.0f;
                            var fontForLabel = _font;
                            
                            var textOptions = new RichTextOptions(fontForLabel) // Use RichTextOptions
                            {
                                Origin = new PointF(xMin + 5, labelY),
                                HorizontalAlignment = HorizontalAlignment.Left
                            };

                            // Create a more visible background for the label
                            float padding = 5.0f * fontSizeAdjustment;
                            var textMeasurement = TextMeasurer.MeasureBounds(label, textOptions);
                            var backgroundRect = new RectangleF(
                                xMin, labelY - padding,
                                textMeasurement.Width + (padding * 2), 
                                textMeasurement.Height + (padding * 2));
                            
                            // Draw background with semi-transparency
                            var rgba = boxColor.ToPixel<Rgba32>(); // Get the RGBA components
                            var bgColor = new Rgba32(rgba.R, rgba.G, rgba.B, 200); // Create new color with alpha 200
                            image.Mutate(ctx => ctx.Fill(bgColor, backgroundRect));
                            
                            // Draw text with a contrasting color
                            var textColor = Color.White;
                            image.Mutate(ctx => ctx.DrawText(textOptions, label, textColor));
                        }
                    }

                    // Save the processed image to a memory stream
                    using (var ms = new MemoryStream())
                    {
                        // Always save as JPEG with good quality
                        await image.SaveAsJpegAsync(ms, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder { Quality = 90 });
                        _logger.LogInformation("Successfully processed image and drew {Count} bounding boxes.", predictions.Count);
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing image or drawing bounding boxes.");
                return null; // Indicate failure
            }
        }
    }
}