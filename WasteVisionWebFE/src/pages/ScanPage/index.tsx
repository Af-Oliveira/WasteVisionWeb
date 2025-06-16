import React, { useState, useEffect, useCallback } from "react";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { LoadingSpinner } from "@/components/ui/loading-spinner";
import {
  ImageCaptureRoot,
  ImageCaptureUploadTrigger,
  ImageCaptureCameraTrigger,
  ImageCapturePreview,
  ImageCaptureReset,
} from "@/components/ui/image-capture";
import {
  CheckCircle,
  AlertTriangle,
  Sparkles,
  RotateCcw,
  Settings2,
  ScanLine,
} from "lucide-react";
import {
  useRoboflowModelService,
  usePredictionService,
  useObjectPredictionService,
} from "@/di/container";
import { useToast } from "@/hooks/useToast";
import { RoboflowModel } from "@/core/domain/RoboflowModel";
import { ObjectPredictionSearchParamsDTO } from "@/data/dto/objectPrediction-dto";
import { ObjectPrediction } from "@/core/domain/ObjectPrediction";
import { Prediction } from "@/core/domain/Prediction";

export function ScanPage() {
  const [selectedModelId, setSelectedModelId] = useState<string>("");
  const [models, setModels] = useState<RoboflowModel[]>([]);
  const [isLoadingModels, setIsLoadingModels] = useState<boolean>(true);
  const [isProcessing, setIsProcessing] = useState<boolean>(false);

  const [prediction, setPrediction] = useState<Prediction | null>(null);
  const [objectPredictions, setObjectPredictions] = useState<
    ObjectPrediction[]
  >([]);

  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [imageDataUrl, setImageDataUrl] = useState<string | null>(null);

  const roboflowModelService = useRoboflowModelService();
  const predictionService = usePredictionService();
  const objectPredictionService = useObjectPredictionService();
  const { success, error, info } = useToast();

  const loadModels = useCallback(async () => {
    try {
      const activeModels =
        await roboflowModelService.getAllActiveRoboflowModels();
      setModels(activeModels);

      if (activeModels.length > 0 && activeModels[0].id) {
        setSelectedModelId(activeModels[0].id);
      } else {
        info("No active detection models found. Please configure models.");
      }
    } catch (err) {
      console.error("Failed to load models:", err);
      error("Failed to load detection models. Please try again.");
    } finally {
      setIsLoadingModels(false);
    }
  }, [roboflowModelService, error, info]);

  useEffect(() => {
    loadModels();
  }, []);

  const handleImageSelected = useCallback(
    (file: File | null, dataUrl: string | null) => {
      setSelectedFile(file);
      setImageDataUrl(dataUrl);
      // Clear previous prediction and object detection results when a new image is selected
      // or when the current image selection is cleared. This prevents displaying stale data.
      setPrediction(null);
      setObjectPredictions([]);
    },
    [] // setPrediction and setObjectPredictions are stable and don't need to be dependencies.
  );

  const handleSubmit = async () => {
    if (!selectedModelId) {
      error("Please select a detection model.");
      return;
    }

    if (!selectedFile) {
      error("Please upload an image or capture one using the camera.");
      return;
    }

    setIsProcessing(true);
    // Ensure previous results are cleared before starting a new detection
    setPrediction(null);
    setObjectPredictions([]);

    try {
      const newPrediction = await predictionService.uploadAndDetect(
        selectedFile,
        selectedModelId
      );
      setPrediction(newPrediction);

      if (newPrediction?.id) {
        const objectPredictionDto: ObjectPredictionSearchParamsDTO = {
          predictionId: newPrediction.id,
        };
        const newObjectPredictions =
          await objectPredictionService.getObjectPredictions(
            objectPredictionDto
          );
        setObjectPredictions(newObjectPredictions);
        success("Detection completed successfully!");
      } else {
        error("Prediction ID not found, cannot fetch object details.");
        // Ensure objectPredictions remains empty if prediction ID is missing
        setObjectPredictions([]);
      }
    } catch (err) {
      console.error("Image processing failed:", err);
      error("Image processing failed. Please try again.");
      setPrediction(null);
      setObjectPredictions([]); // Ensure cleared on error
    } finally {
      setIsProcessing(false);
    }
  };

  const handleResetScan = () => {
    // This function is called when "Start New Scan" is clicked.
    setPrediction(null);
    setObjectPredictions([]); // This line ensures the detection results (objectPredictions) are cleared.
    setIsProcessing(false);
    setSelectedFile(null);
    setImageDataUrl(null);
    // Optionally, you might want to reset the model selection too,
    // but that depends on the desired UX.
    // if (models.length > 0 && models[0].id) {
    //   setSelectedModelId(models[0].id);
    // }
  };

  return (
    <div className="container mx-auto min-h-[calc(100vh-8rem)] px-4 py-8">
      <Card className="w-full max-w-4xl mx-auto shadow-2xl bg-card ring-1 ring-border/20">
        <CardHeader className="border-b border-border/50 bg-muted/20 p-6">
          <div className="flex items-center space-x-3">
            <ScanLine className="h-8 w-8 text-primary" />
            <div>
              <CardTitle className="text-2xl font-bold text-foreground">
                Waste Detection Scanner
              </CardTitle>
              <CardDescription className="text-muted-foreground">
                Upload an image or use your camera to detect waste items.
              </CardDescription>
            </div>
          </div>
        </CardHeader>

        <CardContent className="p-6 md:p-8">
          {/* Configuration and Input Section */}
          {!prediction && (
            <div className="space-y-6">
              {/* Model Selection */}
              <div>
                <label
                  htmlFor="model-select"
                  className="mb-2 flex items-center text-sm font-medium text-foreground"
                >
                  <Settings2 className="mr-2 h-4 w-4 text-primary" />
                  Detection Model
                </label>
                {isLoadingModels ? (
                  <div className="flex items-center space-x-2 rounded-md border p-3">
                    <LoadingSpinner className="h-5 w-5" />
                    <span className="text-muted-foreground">
                      Loading models...
                    </span>
                  </div>
                ) : (
                  <Select
                    value={selectedModelId}
                    onValueChange={setSelectedModelId}
                    disabled={isProcessing || models.length === 0}
                  >
                    <SelectTrigger
                      id="model-select"
                      className="w-full bg-background text-base py-3"
                    >
                      <SelectValue placeholder="Select a model" />
                    </SelectTrigger>
                    <SelectContent>
                      {models.length > 0 ? (
                        models.map((model) => (
                          <SelectItem
                            key={model.id}
                            value={model.id ?? ""}
                            className="text-base"
                          >
                            {model.description || `Model ID: ${model.id}`}
                          </SelectItem>
                        ))
                      ) : (
                        <div className="p-4 text-center text-sm text-muted-foreground">
                          No models available.
                        </div>
                      )}
                    </SelectContent>
                  </Select>
                )}
              </div>

              {/* Image Capture Component */}
              <ImageCaptureRoot
                onImageSelected={handleImageSelected}
                isProcessing={isProcessing}
              >
                {/* Image Source Selection */}
                <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                  <ImageCaptureUploadTrigger className="py-6 text-base transition-all hover:bg-primary/5 hover:text-primary" />
                  <ImageCaptureCameraTrigger className="py-6 text-base transition-all hover:bg-primary/5 hover:text-primary" />
                </div>

                {/* Preview Area */}
                <ImageCapturePreview minHeight="300px" />

                {/* Action Buttons */}
                <div className="flex flex-col gap-4 sm:flex-row">
                  <Button
                    onClick={handleSubmit}
                    disabled={
                      isProcessing ||
                      isLoadingModels ||
                      !selectedModelId ||
                      !selectedFile
                    }
                    className="w-full flex-1 py-3 text-lg"
                  >
                    {isProcessing ? (
                      <LoadingSpinner className="mr-2 h-5 w-5" />
                    ) : (
                      <Sparkles className="mr-2 h-5 w-5" />
                    )}
                    {isProcessing ? "Analyzing..." : "Analyze Waste"}
                  </Button>
                  <ImageCaptureReset className="w-full flex-shrink-0 sm:w-auto py-3 text-lg" />
                </div>
              </ImageCaptureRoot>
            </div>
          )}

          {/* Results Section */}
          {prediction && (
            <div className="space-y-6">
              <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
                {/* Processed Image */}
                <div className="space-y-3">
                  <h3 className="text-xl font-semibold text-foreground">
                    Processed Image
                  </h3>
                  <div className="rounded-lg border overflow-hidden shadow-md">
                    <img
                      src={
                        prediction.processedImageUrl ||
                        prediction.originalImageUrl
                      }
                      alt="Processed waste detection"
                      className="w-full object-contain"
                    />
                  </div>
                </div>

                {/* Detection Details */}
                <div className="space-y-3">
                  <h3 className="text-xl font-semibold text-foreground">
                    Detection Results
                  </h3>
                  {isProcessing && !objectPredictions.length ? (
                    <div className="flex items-center justify-center rounded-md border bg-muted/30 p-8">
                      <LoadingSpinner className="mr-3 h-6 w-6" />
                      <p className="text-muted-foreground">
                        Loading details...
                      </p>
                    </div>
                  ) : objectPredictions.length > 0 ? (
                    <ul className="max-h-[400px] space-y-1 overflow-y-auto rounded-md border bg-muted/10 p-1 shadow-inner">
                      {objectPredictions.map((objPred, index) => (
                        <li
                          key={objPred.id || index}
                          className="flex items-center justify-between rounded-md p-3 transition-colors hover:bg-primary/5"
                        >
                          <div className="flex items-center">
                            <CheckCircle className="mr-3 h-5 w-5 flex-shrink-0 text-green-500" />
                            <span className="font-medium text-foreground">
                              {objPred.category}
                            </span>
                          </div>
                          <span
                            className={`ml-2 rounded-full px-2.5 py-0.5 text-xs font-semibold ${
                              Number(objPred.confidence) > 0.7
                                ? "bg-green-100 text-green-700 dark:bg-green-700 dark:text-green-100"
                                : Number(objPred.confidence) > 0.4
                                ? "bg-yellow-100 text-yellow-700 dark:bg-yellow-700 dark:text-yellow-100"
                                : "bg-red-100 text-red-700 dark:bg-red-700 dark:text-red-100"
                            }`}
                          >
                            {`${(Number(objPred.confidence) * 100).toFixed(
                              1
                            )}%`}
                          </span>
                        </li>
                      ))}
                    </ul>
                  ) : (
                    <div className="flex flex-col items-center justify-center rounded-md border border-dashed p-8 text-center">
                      <AlertTriangle className="mb-3 h-10 w-10 text-amber-500" />
                      <p className="text-lg font-medium text-muted-foreground">
                        No objects detected.
                      </p>
                      <p className="text-sm text-muted-foreground">
                        The model couldn't identify any items in this image.
                      </p>
                    </div>
                  )}
                </div>
              </div>
              <CardFooter className="mt-4 border-t border-border/50 bg-muted/20 p-6">
                <Button
                  onClick={handleResetScan}
                  className="w-full py-3 text-lg sm:w-auto sm:px-8"
                >
                  <RotateCcw className="mr-2 h-5 w-5" /> Start New Scan
                </Button>
              </CardFooter>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
