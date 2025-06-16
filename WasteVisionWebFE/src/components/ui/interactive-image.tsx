// src/components/ui/interactive-image.tsx
import React, { useState, useRef, useEffect } from 'react';
import { ObjectPrediction } from '@/core/domain/ObjectPrediction';

interface InteractiveImageProps {
  imageUrl: string;
  objectPredictions: ObjectPrediction[];
  selectedObject?: ObjectPrediction | null;
  onObjectSelect?: (object: ObjectPrediction | null) => void;
  className?: string;
}

export const InteractiveImage: React.FC<InteractiveImageProps> = ({
  imageUrl,
  objectPredictions,
  selectedObject,
  onObjectSelect,
  className = "",
}) => {
  const containerRef = useRef<HTMLDivElement>(null);
  const imageRef = useRef<HTMLImageElement>(null);
  const [imageLoaded, setImageLoaded] = useState(false);
  const [naturalDimensions, setNaturalDimensions] = useState({ 
    width: 0, 
    height: 0 
  });
  const [imageBounds, setImageBounds] = useState({ 
    width: 0, 
    height: 0, 
    left: 0, 
    top: 0 
  });

  // Update image bounds when image loads or container resizes
  const updateImageBounds = () => {
    if (imageRef.current && containerRef.current) {
      const imageElement = imageRef.current;
      const containerElement = containerRef.current;
      
      // Get the actual rendered dimensions and position of the image
      const imageRect = imageElement.getBoundingClientRect();
      const containerRect = containerElement.getBoundingClientRect();
      
      // Calculate the position and size of the image within the container
      setImageBounds({
        width: imageRect.width,
        height: imageRect.height,
        left: imageRect.left - containerRect.left,
        top: imageRect.top - containerRect.top,
      });
    }
  };

  useEffect(() => {
    const img = new Image();
    img.onload = () => {
      setNaturalDimensions({
        width: img.naturalWidth,
        height: img.naturalHeight,
      });
      setImageLoaded(true);
      // Use setTimeout to ensure DOM is fully updated after image load
      setTimeout(updateImageBounds, 100);
    };
    img.src = imageUrl;
  }, [imageUrl]);

  useEffect(() => {
    if (!imageLoaded) return;

    // Update bounds when image loads or container changes
    updateImageBounds();

    // Use ResizeObserver for container size changes
    let resizeObserver: ResizeObserver | null = null;
    
    if (containerRef.current) {
      resizeObserver = new ResizeObserver(() => {
        setTimeout(updateImageBounds, 10);
      });
      resizeObserver.observe(containerRef.current);
    }

    // Handle window resize
    const handleResize = () => {
      setTimeout(updateImageBounds, 10);
    };
    window.addEventListener('resize', handleResize);
    
    return () => {
      window.removeEventListener('resize', handleResize);
      if (resizeObserver) {
        resizeObserver.disconnect();
      }
    };
  }, [imageLoaded]);

  const getScaledCoordinates = (obj: ObjectPrediction) => {
    if (!imageLoaded || !naturalDimensions.width || !imageBounds.width) {
      return null;
    }

    // Parse the coordinates
    let x = parseFloat(obj.x);
    let y = parseFloat(obj.y);
    let width = parseFloat(obj.width);
    let height = parseFloat(obj.height);

    // Calculate scale factors
    const scaleX = imageBounds.width / naturalDimensions.width;
    const scaleY = imageBounds.height / naturalDimensions.height;

    // Determine if coordinates are normalized (0-1 range)
    const isNormalized = x <= 1 && y <= 1 && width <= 1 && height <= 1;

    if (isNormalized) {
      // Coordinates are normalized (0-1), convert to natural image pixels first
      x = x * naturalDimensions.width;
      y = y * naturalDimensions.height;
      width = width * naturalDimensions.width;
      height = height * naturalDimensions.height;
    }

    // At this point, coordinates are in natural image pixel space
    // Check if they are center-based (common in YOLO/Roboflow)
    // We can detect this by checking if x,y seem to be center coordinates
    const isCenterBased = (x > width/2) && (y > height/2) && 
                         (x + width/2 <= naturalDimensions.width) && 
                         (y + height/2 <= naturalDimensions.height);

    if (isCenterBased) {
      // Convert from center-based to top-left
      x = x - width / 2;
      y = y - height / 2;
    }

    // Ensure coordinates are within bounds
    x = Math.max(0, Math.min(x, naturalDimensions.width - width));
    y = Math.max(0, Math.min(y, naturalDimensions.height - height));
    width = Math.min(width, naturalDimensions.width - x);
    height = Math.min(height, naturalDimensions.height - y);

    // Scale to displayed image size
    x = x * scaleX;
    y = y * scaleY;
    width = width * scaleX;
    height = height * scaleY;

    return { x, y, width, height };
  };

  return (
    <div ref={containerRef} className={`relative inline-block ${className}`}>
      <img
        ref={imageRef}
        src={imageUrl}
        alt="Prediction result"
        className="w-full h-auto max-w-full max-h-[600px] object-contain rounded-lg"
        onLoad={() => {
          setImageLoaded(true);
          setTimeout(updateImageBounds, 100);
        }}
        style={{ display: 'block' }}
      />

     

      {/* Object Overlays */}
      {imageLoaded && naturalDimensions.width > 0 && imageBounds.width > 0 && 
        objectPredictions.map((obj) => {
          const coords = getScaledCoordinates(obj);
          if (!coords) return null;

          const isSelected = selectedObject?.id === obj.id;

          return (
            <div
              key={obj.id}
              className={`absolute border-2 cursor-pointer transition-all duration-200 ${
                isSelected
                  ? 'border-red-500 bg-red-500/20'
                  : 'border-green-500 bg-green-500/10 hover:bg-green-500/20'
              }`}
              style={{
                left: `${imageBounds.left + coords.x}px`,
                top: `${imageBounds.top + coords.y}px`,
                width: `${coords.width}px`,
                height: `${coords.height}px`,
                pointerEvents: 'auto',
                zIndex: 10,
              }}
              onClick={() => onObjectSelect?.(isSelected ? null : obj)}
            >
              {/* Category Label */}
              <div
                className={`absolute px-2 py-1 text-xs font-medium rounded whitespace-nowrap z-10 ${
                  isSelected
                    ? 'bg-red-500 text-white'
                    : 'bg-green-500 text-white'
                }`}
                style={{
                  left: '0px',
                  top: coords.y < 30 ? `${coords.height + 4}px` : '-28px',
                }}
              >
                {obj.category}
              </div>

              {/* Confidence Badge */}
              <div
                className={`absolute px-2 py-1 text-xs font-medium rounded whitespace-nowrap z-10 ${
                  isSelected
                    ? 'bg-red-500 text-white'
                    : 'bg-blue-500 text-white'
                }`}
                style={{
                  right: '0px',
                  bottom: coords.y + coords.height > imageBounds.height - 30 
                    ? `${coords.height + 4}px`
                    : '-28px',
                }}
              >
                {parseFloat(obj.confidence).toFixed(2)}%
              </div>
            </div>
          );
        })}
    </div>
  );
};
