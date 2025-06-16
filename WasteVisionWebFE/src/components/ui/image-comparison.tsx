// src/components/ui/image-comparison.tsx
import React, { useRef, useState, useEffect } from 'react';

interface ImageComparisonProps {
  beforeImage: string;
  afterImage: string;
  beforeLabel?: string;
  afterLabel?: string;
  className?: string;
  height?: string;
}

export const ImageComparison: React.FC<ImageComparisonProps> = ({
  beforeImage,
  afterImage,
  beforeLabel = "Original",
  afterLabel = "Processed",
  className = "",
  height = "600px",
}) => {
  const containerRef = useRef<HTMLDivElement>(null);
  const [sliderPosition, setSliderPosition] = useState(50);
  const [isDragging, setIsDragging] = useState(false);
  const [imagesLoaded, setImagesLoaded] = useState({ before: false, after: false });
  const [imageDimensions, setImageDimensions] = useState({ width: 0, height: 0 });

  useEffect(() => {
    // Load both images and determine the best fit dimensions
    const loadImages = async () => {
      const beforeImg = new Image();
      const afterImg = new Image();
      
      const beforePromise = new Promise<{ width: number; height: number }>((resolve) => {
        beforeImg.onload = () => resolve({ width: beforeImg.naturalWidth, height: beforeImg.naturalHeight });
      });
      
      const afterPromise = new Promise<{ width: number; height: number }>((resolve) => {
        afterImg.onload = () => resolve({ width: afterImg.naturalWidth, height: afterImg.naturalHeight });
      });

      beforeImg.src = beforeImage;
      afterImg.src = afterImage;

      try {
        const [beforeDims, afterDims] = await Promise.all([beforePromise, afterPromise]);
        
        // Use the dimensions from the first image (they should be the same for before/after)
        const targetWidth = Math.max(beforeDims.width, afterDims.width);
        const targetHeight = Math.max(beforeDims.height, afterDims.height);
        
        setImageDimensions({ width: targetWidth, height: targetHeight });
        setImagesLoaded({ before: true, after: true });
      } catch (error) {
        console.error('Error loading images:', error);
      }
    };

    loadImages();
  }, [beforeImage, afterImage]);

  const handleMouseDown = () => {
    setIsDragging(true);
  };

  const handleMouseUp = () => {
    setIsDragging(false);
  };

  const handleMouseMove = (e: React.MouseEvent) => {
    if (!isDragging || !containerRef.current) return;

    const rect = containerRef.current.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const percentage = (x / rect.width) * 100;
    setSliderPosition(Math.max(0, Math.min(100, percentage)));
  };

  useEffect(() => {
    const handleGlobalMouseUp = () => setIsDragging(false);
    const handleGlobalMouseMove = (e: MouseEvent) => {
      if (!isDragging || !containerRef.current) return;

      const rect = containerRef.current.getBoundingClientRect();
      const x = e.clientX - rect.left;
      const percentage = (x / rect.width) * 100;
      setSliderPosition(Math.max(0, Math.min(100, percentage)));
    };

    if (isDragging) {
      document.addEventListener('mousemove', handleGlobalMouseMove);
      document.addEventListener('mouseup', handleGlobalMouseUp);
    }

    return () => {
      document.removeEventListener('mousemove', handleGlobalMouseMove);
      document.removeEventListener('mouseup', handleGlobalMouseUp);
    };
  }, [isDragging]);

  if (!imagesLoaded.before || !imagesLoaded.after) {
    return (
      <div className={`${className} flex items-center justify-center bg-muted rounded-lg`} style={{ height }}>
        <div className="text-muted-foreground">Loading images...</div>
      </div>
    );
  }

  return (
    <div
      ref={containerRef}
      className={`relative overflow-hidden rounded-lg bg-black ${className}`}
      style={{ 
        height,
        userSelect: 'none',
      }}
      onMouseMove={handleMouseMove}
      onMouseUp={handleMouseUp}
    >
      {/* After Image (Full) - Base layer */}
      <img
        src={afterImage}
        alt={afterLabel}
        className="absolute inset-0 w-full h-full object-contain"
        draggable={false}
        style={{ 
          objectFit: 'contain',
          objectPosition: 'center',
        }}
      />

      {/* Before Image (Clipped) - Overlay layer */}
      <div
        className="absolute inset-0 overflow-hidden"
        style={{ 
          clipPath: `inset(0 ${100 - sliderPosition}% 0 0)`,
        }}
      >
        <img
          src={beforeImage}
          alt={beforeLabel}
          className="absolute inset-0 w-full h-full object-contain"
          draggable={false}
          style={{ 
            objectFit: 'contain',
            objectPosition: 'center',
          }}
        />
      </div>

      {/* Slider Line */}
      <div
        className="absolute inset-y-0 w-0.5 bg-white shadow-lg cursor-ew-resize z-10"
        style={{ left: `${sliderPosition}%` }}
        onMouseDown={handleMouseDown}
      >
        {/* Slider Handle */}
        <div className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-8 h-8 bg-white rounded-full shadow-lg flex items-center justify-center cursor-ew-resize">
          <div className="flex gap-0.5">
            <div className="w-0.5 h-4 bg-gray-400"></div>
            <div className="w-0.5 h-4 bg-gray-400"></div>
          </div>
        </div>
      </div>

      {/* Labels */}
      <div className="absolute top-4 left-4 bg-black/70 text-white text-sm px-3 py-1 rounded z-10">
        {beforeLabel}
      </div>
      <div className="absolute top-4 right-4 bg-black/70 text-white text-sm px-3 py-1 rounded z-10">
        {afterLabel}
      </div>

      {/* Instructions */}
      <div className="absolute bottom-4 left-1/2 transform -translate-x-1/2 bg-black/70 text-white text-xs px-3 py-1 rounded z-10">
        Drag to compare
      </div>
    </div>
  );
};
