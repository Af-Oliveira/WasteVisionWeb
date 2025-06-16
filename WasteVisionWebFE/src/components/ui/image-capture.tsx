import * as React from "react";
import { Button } from "@/components/ui/button";
import {
  Camera,
  Upload,
  XCircle,
  Video,
  Image as ImageIcon,
  RotateCcw,
} from "lucide-react";
import { cn } from "@/lib/utils";

// Types
interface ImageCaptureContextType {
  selectedFile: File | null;
  setSelectedFile: (file: File | null) => void;
  imagePreviewUrl: string | null;
  setImagePreviewUrl: (url: string | null) => void;
  capturedImage: string | null;
  setCapturedImage: (image: string | null) => void;
  isCameraActive: boolean;
  setIsCameraActive: (active: boolean) => void;
  isProcessing: boolean;
  setIsProcessing: (processing: boolean) => void;
  onImageSelected?: (file: File | null, dataUrl: string | null) => void;
  reset: () => void;
}

// Context
const ImageCaptureContext = React.createContext<
  ImageCaptureContextType | undefined
>(undefined);

const useImageCapture = () => {
  const context = React.useContext(ImageCaptureContext);
  if (!context) {
    throw new Error(
      "useImageCapture must be used within an ImageCaptureRoot"
    );
  }
  return context;
};

// Helper function
const dataURLtoFile = (dataurl: string, filename: string): File | null => {
  try {
    const arr = dataurl.split(",");
    if (arr.length < 2) return null;
    const mimeMatch = arr[0].match(/:(.*?);/);
    if (!mimeMatch) return null;
    const mime = mimeMatch[1];
    const bstr = atob(arr[1]);
    let n = bstr.length;
    const u8arr = new Uint8Array(n);
    while (n--) {
      u8arr[n] = bstr.charCodeAt(n);
    }
    return new File([u8arr], filename, { type: mime });
  } catch (e) {
    console.error("Error converting data URL to file:", e);
    return null;
  }
};

// Root Component
interface ImageCaptureRootProps {
  children: React.ReactNode;
  onImageSelected?: (file: File | null, dataUrl: string | null) => void;
  isProcessing?: boolean;
}

const ImageCaptureRoot = React.forwardRef<
  HTMLDivElement,
  ImageCaptureRootProps
>(({ children, onImageSelected, isProcessing = false }, ref) => {
  const [selectedFile, setSelectedFile] = React.useState<File | null>(null);
  const [imagePreviewUrl, setImagePreviewUrl] = React.useState<string | null>(
    null
  );
  const [capturedImage, setCapturedImage] = React.useState<string | null>(null);
  const [isCameraActive, setIsCameraActive] = React.useState<boolean>(false);
  const [internalProcessing, setInternalProcessing] =
    React.useState<boolean>(false);

  const reset = React.useCallback(() => {
    setSelectedFile(null);
    setImagePreviewUrl(null);
    setCapturedImage(null);
    setIsCameraActive(false);
    setInternalProcessing(false);
  }, []);

  // Notify parent when image changes
  React.useEffect(() => {
    if (onImageSelected) {
      const file = capturedImage
        ? dataURLtoFile(capturedImage, "capture.jpeg")
        : selectedFile;
      const dataUrl = imagePreviewUrl || capturedImage;
      onImageSelected(file, dataUrl);
    }
  }, [selectedFile, capturedImage, imagePreviewUrl, onImageSelected]);

  const contextValue: ImageCaptureContextType = {
    selectedFile,
    setSelectedFile,
    imagePreviewUrl,
    setImagePreviewUrl,
    capturedImage,
    setCapturedImage,
    isCameraActive,
    setIsCameraActive,
    isProcessing: isProcessing || internalProcessing,
    setIsProcessing: setInternalProcessing,
    onImageSelected,
    reset,
  };

  return (
    <ImageCaptureContext.Provider value={contextValue}>
      <div ref={ref} className="space-y-4">
        {children}
      </div>
    </ImageCaptureContext.Provider>
  );
});
ImageCaptureRoot.displayName = "ImageCaptureRoot";

// Trigger Components
interface ImageCaptureTriggerProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: "outline" | "default" | "destructive" | "secondary" | "ghost" | "link";
}

const ImageCaptureUploadTrigger = React.forwardRef<
  HTMLButtonElement,
  ImageCaptureTriggerProps
>(({ className, variant = "outline", children, ...props }, ref) => {
  const {
    setSelectedFile,
    setImagePreviewUrl,
    setCapturedImage,
    setIsCameraActive,
    isCameraActive,
    isProcessing,
  } = useImageCapture();
  const fileInputRef = React.useRef<HTMLInputElement>(null);

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      if (!file.type.startsWith("image/")) {
        console.error("Invalid file type. Please select an image.");
        return;
      }
      // Reset other sources
      setCapturedImage(null);
      if (isCameraActive) {
        setIsCameraActive(false);
      }

      setSelectedFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setImagePreviewUrl(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleClick = () => {
    fileInputRef.current?.click();
  };

  return (
    <>
      <Button
        ref={ref}
        variant={variant}
        className={cn("w-full", className)}
        onClick={handleClick}
        disabled={isProcessing || isCameraActive}
        {...props}
      >
        {children || (
          <>
            <Upload className="mr-2 h-4 w-4" />
            Upload Image
          </>
        )}
      </Button>
      <input
        type="file"
        ref={fileInputRef}
        className="hidden"
        accept="image/*"
        onChange={handleFileSelect}
        disabled={isProcessing}
      />
    </>
  );
});
ImageCaptureUploadTrigger.displayName = "ImageCaptureUploadTrigger";

const ImageCaptureCameraTrigger = React.forwardRef<
  HTMLButtonElement,
  ImageCaptureTriggerProps
>(({ className, variant = "outline", children, ...props }, ref) => {
  const {
    setIsCameraActive,
    setSelectedFile,
    setImagePreviewUrl,
    isCameraActive,
    isProcessing,
  } = useImageCapture();

  const handleInitializeCamera = async () => {
    // Reset other sources
    setSelectedFile(null);
    setImagePreviewUrl(null);
    setIsCameraActive(true);
  };

  return (
    <Button
      ref={ref}
      variant={variant}
      className={cn("w-full", className)}
      onClick={handleInitializeCamera}
      disabled={isProcessing}
      {...props}
    >
      {children || (
        <>
          <Camera className="mr-2 h-4 w-4" />
          {isCameraActive ? "Camera Active" : "Use Camera"}
        </>
      )}
    </Button>
  );
});
ImageCaptureCameraTrigger.displayName = "ImageCaptureCameraTrigger";

// Preview Component
interface ImageCapturePreviewProps
  extends React.HTMLAttributes<HTMLDivElement> {
  minHeight?: string;
}

const ImageCapturePreview = React.forwardRef<
  HTMLDivElement,
  ImageCapturePreviewProps
>(({ className, minHeight = "300px", ...props }, ref) => {
  const {
    selectedFile,
    imagePreviewUrl,
    capturedImage,
    isCameraActive,
    setIsCameraActive,
    setCapturedImage,
    setSelectedFile,
    setImagePreviewUrl,
    isProcessing,
  } = useImageCapture();

  const videoRef = React.useRef<HTMLVideoElement>(null);
  const streamRef = React.useRef<MediaStream | null>(null);

  const stopCameraStream = React.useCallback(() => {
    if (streamRef.current) {
      streamRef.current.getTracks().forEach((track) => track.stop());
      streamRef.current = null;
    }
    if (videoRef.current) {
      videoRef.current.srcObject = null;
    }
  }, []);

  // Initialize camera when active
  React.useEffect(() => {
    if (isCameraActive) {
      const initCamera = async () => {
        try {
          const constraints = {
            video: {
              facingMode: "environment",
              width: { ideal: 1920, max: 1920 },
              height: { ideal: 1080, max: 1080 },
            },
            audio: false,
          };

          const stream = await navigator.mediaDevices.getUserMedia(constraints);
          streamRef.current = stream;

          if (videoRef.current) {
            videoRef.current.srcObject = stream;
            
            // Wait for metadata to load before playing
            videoRef.current.onloadedmetadata = () => {
              videoRef.current?.play().catch((err) => {
                console.error("Video play failed:", err);
                setIsCameraActive(false);
              });
            };
          }
        } catch (err) {
          console.error("Failed to initialize camera:", err);
          setIsCameraActive(false);
        }
      };

      initCamera();
    } else {
      stopCameraStream();
    }

    return () => {
      stopCameraStream();
    };
  }, [isCameraActive, setIsCameraActive, stopCameraStream]);

  const handleCaptureImage = () => {
    if (!videoRef.current || !streamRef.current) {
      console.error("Camera stream not available.");
      return;
    }

    const canvas = document.createElement("canvas");
    const video = videoRef.current;
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    const ctx = canvas.getContext("2d");

    if (ctx) {
      ctx.drawImage(video, 0, 0, canvas.width, canvas.height);
      const imageDataUrl = canvas.toDataURL("image/jpeg", 0.9);
      setCapturedImage(imageDataUrl);
      setSelectedFile(null);
      setImagePreviewUrl(null);
    } else {
      console.error("Failed to capture image from camera.");
    }
    setIsCameraActive(false);
  };

  const handleClearImage = () => {
    setSelectedFile(null);
    setImagePreviewUrl(null);
    setCapturedImage(null);
    if (isCameraActive) {
      setIsCameraActive(false);
    }
  };

  const currentImageToDisplay = imagePreviewUrl || capturedImage;

  return (
    <div
      ref={ref}
      className={cn(
        "relative rounded-lg border-2 border-dashed border-border bg-muted/20 flex items-center justify-center overflow-hidden",
        className
      )}
      style={{ minHeight }}
      {...props}
    >
      {isCameraActive && (
        <>
          <video
            ref={videoRef}
            className="h-full w-full object-cover"
            playsInline
            muted
          />
          <Button
            onClick={handleCaptureImage}
            className="absolute bottom-4 left-1/2 z-10 -translate-x-1/2 transform px-6 py-3 shadow-lg"
            disabled={isProcessing}
          >
            <Video className="mr-2 h-5 w-5" /> Capture
          </Button>
        </>
      )}

      {!isCameraActive && currentImageToDisplay && (
        <>
          <img
            src={currentImageToDisplay}
            alt="Preview"
            className="max-h-full w-auto object-contain rounded-md"
          />
          <Button
            onClick={handleClearImage}
            variant="destructive"
            size="icon"
            className="absolute top-3 right-3 z-10 rounded-full shadow-lg"
            title="Clear Image"
          >
            <XCircle className="h-5 w-5" />
          </Button>
        </>
      )}

      {!isCameraActive && !currentImageToDisplay && (
        <div className="text-center text-muted-foreground">
          <ImageIcon className="mx-auto mb-2 h-12 w-12" />
          <p>Image preview will appear here</p>
        </div>
      )}
    </div>
  );
});
ImageCapturePreview.displayName = "ImageCapturePreview";

// Reset Button Component
const ImageCaptureReset = React.forwardRef<
  HTMLButtonElement,
  ImageCaptureTriggerProps
>(({ className, variant = "outline", children, ...props }, ref) => {
  const { reset, isProcessing } = useImageCapture();

  return (
    <Button
      ref={ref}
      variant={variant}
      className={cn(className)}
      onClick={reset}
      disabled={isProcessing}
      {...props}
    >
      {children || (
        <>
          <RotateCcw className="mr-2 h-4 w-4" />
          Reset
        </>
      )}
    </Button>
  );
});
ImageCaptureReset.displayName = "ImageCaptureReset";

// Export all components
export {
  ImageCaptureRoot,
  ImageCaptureUploadTrigger,
  ImageCaptureCameraTrigger,
  ImageCapturePreview,
  ImageCaptureReset,
  useImageCapture,
};

