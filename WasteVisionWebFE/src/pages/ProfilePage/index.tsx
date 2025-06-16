// src/pages/UserProfilePage.tsx
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { LoadingSpinner } from "@/components/ui/loading-spinner";
import { useAuth } from "@/hooks/useAuth";
import {
  Camera,
  Eye,
  Target,
  Calendar,
  User as UserIcon,
  Mail,
  Shield,
  ArrowLeft,
  Grid,
  List,
  Maximize2,
  X,
} from "lucide-react";
import { useEffect, useState, useCallback } from "react";
import {
  usePredictionService,
  useObjectPredictionService,
} from "@/di/container";
import { Prediction } from "@/core/domain/Prediction";
import { ObjectPrediction } from "@/core/domain/ObjectPrediction";
import { ImageComparison } from "@/components/ui/image-comparison";
import { InteractiveImage } from "@/components/ui/interactive-image";

interface UserProfileData {
  predictions: Prediction[];
  objectPredictions: ObjectPrediction[];
}

interface PredictionWithDetails extends Prediction {
  detectionCount: number;
  topCategory?: string;
  objectPredictions: ObjectPrediction[];
}

export function UserProfilePage() {
  const predictionApi = usePredictionService();
  const objectPredictionApi = useObjectPredictionService();
  const { user } = useAuth();

  const [loading, setLoading] = useState(true);
  const [profileData, setProfileData] = useState<UserProfileData>({
    predictions: [],
    objectPredictions: [],
  });
  const [viewMode, setViewMode] = useState<"grid" | "list">("grid");
  const [selectedPrediction, setSelectedPrediction] = useState<PredictionWithDetails | null>(null);
  const [selectedObject, setSelectedObject] = useState<ObjectPrediction | null>(null);
  const [comparisonMode, setComparisonMode] = useState<"comparison" | "interactive">("comparison");

  const loadUserProfileData = useCallback(async () => {
    if (!user?.id) return;

    try {
      setLoading(true);

      const [predictions, objectPredictions] = await Promise.all([
        predictionApi.getPredictions({ userId: user.id }),
        objectPredictionApi.getObjectPredictions(),
      ]);

      const userPredictionIds = predictions.map((p) => p.id).filter(Boolean);
      const userObjectPredictions = objectPredictions.filter((obj) =>
        userPredictionIds.includes(obj.predictionId)
      );

      setProfileData({
        predictions,
        objectPredictions: userObjectPredictions,
      });
    } catch (err) {
      console.error("Failed to load user profile data:", err);
    } finally {
      setLoading(false);
    }
  }, [predictionApi, objectPredictionApi, user?.id]);

  useEffect(() => {
    loadUserProfileData();
  }, [loadUserProfileData]);

  const calculateSimpleStats = () => {
    const { predictions, objectPredictions } = profileData;

    const totalPredictions = predictions.length;
    const totalDetections = objectPredictions.length;

    const avgConfidence =
      objectPredictions.length > 0
        ? (
            objectPredictions.reduce(
              (sum, obj) => sum + parseFloat(obj.confidence),
              0
            ) / objectPredictions.length
          ).toFixed(2)
        : "0";

    const memberSince = predictions.length > 0
      ? new Date(
          Math.min(...predictions.map((p) => new Date(p.date).getTime()))
        ).toLocaleDateString()
      : "N/A";

    return {
      totalPredictions,
      totalDetections,
      avgConfidence: `${avgConfidence}%`,
      memberSince,
    };
  };

  const getPredictionsWithDetails = (): PredictionWithDetails[] => {
    const { predictions, objectPredictions } = profileData;

    return predictions
      .map((prediction) => {
        const relatedDetections = objectPredictions.filter(
          (obj) => obj.predictionId === prediction.id
        );

        const categoryCount = relatedDetections.reduce((acc, det) => {
          acc[det.category] = (acc[det.category] || 0) + 1;
          return acc;
        }, {} as Record<string, number>);

        const topCategory = Object.entries(categoryCount).length > 0
          ? Object.entries(categoryCount).reduce((a, b) => a[1] > b[1] ? a : b)[0]
          : undefined;

        return {
          ...prediction,
          detectionCount: relatedDetections.length,
          topCategory,
          objectPredictions: relatedDetections,
        };
      })
      .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());
  };

  const handlePredictionClick = (prediction: PredictionWithDetails) => {
    setSelectedPrediction(prediction);
    setSelectedObject(null);
  };

  const handleBackToList = () => {
    setSelectedPrediction(null);
    setSelectedObject(null);
  };

  if (loading) {
    return (
      <div className="flex flex-1 items-center justify-center min-h-[600px]">
        <LoadingSpinner />
      </div>
    );
  }

  if (!user) {
    return (
      <div className="flex flex-1 items-center justify-center min-h-[600px]">
        <p>Please log in to view your profile.</p>
      </div>
    );
  }

  const stats = calculateSimpleStats();
  const predictionsWithDetails = getPredictionsWithDetails();

  // If a prediction is selected, show the detailed view
  if (selectedPrediction) {
    return (
      <div className="max-w-7xl mx-auto px-4 py-6 w-full">
        {/* Header with Back Button */}
        <div className="flex items-center gap-4 mb-6">
          <button
            onClick={handleBackToList}
            className="flex items-center gap-2 px-4 py-2 bg-muted hover:bg-muted/80 rounded-lg transition-colors"
          >
            <ArrowLeft className="h-4 w-4" />
            Back to Predictions
          </button>
         
        </div>

        <div className="grid gap-6 lg:grid-cols-3">
          {/* Image Comparison/Interactive View */}
          <div className="lg:col-span-2">
            <Card>
              <CardHeader>
                <div className="flex justify-between items-center">
                  <CardTitle>Image Analysis</CardTitle>
                  <div className="flex gap-2">
                    <button
                      onClick={() => setComparisonMode("comparison")}
                      className={`px-3 py-1 rounded text-sm transition-colors ${
                        comparisonMode === "comparison"
                          ? "bg-primary text-primary-foreground"
                          : "bg-muted hover:bg-muted/80"
                      }`}
                    >
                      Compare
                    </button>
                    <button
                      onClick={() => setComparisonMode("interactive")}
                      className={`px-3 py-1 rounded text-sm transition-colors ${
                        comparisonMode === "interactive"
                          ? "bg-primary text-primary-foreground"
                          : "bg-muted hover:bg-muted/80"
                      }`}
                    >
                      Interactive
                    </button>
                  </div>
                </div>
              </CardHeader>
              <CardContent>
                {comparisonMode === "comparison" ? (
                  <ImageComparison
                    beforeImage={selectedPrediction.originalImageUrl}
                    afterImage={selectedPrediction.processedImageUrl}
                    className="w-full h-96"
                  />
                ) : (
                  <InteractiveImage
                    imageUrl={selectedPrediction.originalImageUrl}
                    objectPredictions={selectedPrediction.objectPredictions}
                    selectedObject={selectedObject}
                    onObjectSelect={setSelectedObject}
                    className="w-full"
                  />
                )}
              </CardContent>
            </Card>
          </div>

          {/* Object Predictions List */}
          <div>
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Target className="h-5 w-5" />
                  Detected Objects ({selectedPrediction.objectPredictions.length})
                </CardTitle>
                <CardDescription>
                  Click on an object to highlight it on the image
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="space-y-3 max-h-96 overflow-y-auto">
                  {selectedPrediction.objectPredictions.length === 0 ? (
                    <p className="text-muted-foreground text-center py-4">
                      No objects detected in this image
                    </p>
                  ) : (
                    selectedPrediction.objectPredictions
                      .sort((a, b) => parseFloat(b.confidence) - parseFloat(a.confidence))
                      .map((obj, index) => (
                        <div
                          key={obj.id}
                          className={`p-3 rounded-lg border cursor-pointer transition-all ${
                            selectedObject?.id === obj.id
                              ? "border-primary bg-primary/10"
                              : "border-border hover:border-primary/50 hover:bg-accent/50"
                          }`}
                          onClick={() => setSelectedObject(selectedObject?.id === obj.id ? null : obj)}
                        >
                          <div className="flex justify-between items-start mb-2">
                            <h4 className="font-semibold">{obj.category}</h4>
                            <span className="text-sm bg-green-100 text-green-800 px-2 py-1 rounded dark:bg-green-900/20 dark:text-green-400">
                              {parseFloat(obj.confidence).toFixed(2)}%
                            </span>
                          </div>
                          <div className="text-sm text-muted-foreground space-y-1">
                            <p>Position: ({parseFloat(obj.x).toFixed(0)}, {parseFloat(obj.y).toFixed(0)})</p>
                            <p>Size: {parseFloat(obj.width).toFixed(0)} × {parseFloat(obj.height).toFixed(0)}px</p>
                          </div>
                        </div>
                      ))
                  )}
                </div>
              </CardContent>
            </Card>

            {/* Prediction Info */}
            <Card className="mt-6">
              <CardHeader>
                <CardTitle>Prediction Details</CardTitle>
              </CardHeader>
              <CardContent className="space-y-3">
                <div className="flex justify-between">
                  <span className="text-muted-foreground">Date:</span>
                  <span>{new Date(selectedPrediction.date).toLocaleString()}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-muted-foreground">Model:</span>
                  <span className="font-mono text-sm">{selectedPrediction.roboflowModel?.description}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-muted-foreground">Objects Found:</span>
                  <span>{selectedPrediction.detectionCount}</span>
                </div>
                {selectedPrediction.topCategory && (
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Top Category:</span>
                    <span className="bg-primary/10 text-primary px-2 py-1 rounded text-sm">
                      {selectedPrediction.topCategory}
                    </span>
                  </div>
                )}
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    );
  }

  // Main profile view
  return (
    <div className="max-w-7xl mx-auto px-4 py-6 w-full">
      {/* User Info Header */}
      <Card className="mb-8">
        <CardContent className="pt-6">
          <div className="flex items-center gap-6">
            <div className="bg-primary/10 p-6 rounded-full dark:bg-primary/20">
              <UserIcon className="h-12 w-12 text-primary" />
            </div>
            <div className="flex-1">
              <h1 className="text-3xl font-bold text-foreground mb-2">
                {user.username}
              </h1>
              <div className="flex flex-col md:flex-row gap-4 text-muted-foreground">
                <div className="flex items-center gap-2">
                  <Mail className="h-4 w-4" />
                  <span>{user.email}</span>
                </div>
                <div className="flex items-center gap-2">
                  <Shield className="h-4 w-4" />
                  <span>Role: {user.role?.description || "User"}</span>
                </div>
                <div className="flex items-center gap-2">
                  <Calendar className="h-4 w-4" />
                  <span>Member since: {stats.memberSince}</span>
                </div>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Simple Stats */}
      <div className="grid gap-6 md:grid-cols-3 mb-8">
        <Card>
          <CardContent className="pt-6">
            <div className="flex items-center gap-4">
              <div className="bg-blue-100 p-3 rounded-full dark:bg-blue-900/20">
                <Camera className="h-6 w-6 text-blue-600 dark:text-blue-400" />
              </div>
              <div>
                <p className="text-2xl font-bold">{stats.totalPredictions}</p>
                <p className="text-sm text-muted-foreground">Total Predictions</p>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="pt-6">
            <div className="flex items-center gap-4">
              <div className="bg-green-100 p-3 rounded-full dark:bg-green-900/20">
                <Target className="h-6 w-6 text-green-600 dark:text-green-400" />
              </div>
              <div>
                <p className="text-2xl font-bold">{stats.totalDetections}</p>
                <p className="text-sm text-muted-foreground">Objects Detected</p>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="pt-6">
            <div className="flex items-center gap-4">
              <div className="bg-purple-100 p-3 rounded-full dark:bg-purple-900/20">
                <Eye className="h-6 w-6 text-purple-600 dark:text-purple-400" />
              </div>
              <div>
                <p className="text-2xl font-bold">{stats.avgConfidence}</p>
                <p className="text-sm text-muted-foreground">Avg. Confidence</p>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Predictions List */}
      <Card>
        <CardHeader>
          <div className="flex justify-between items-center">
            <div>
              <CardTitle className="flex items-center gap-2">
                <Camera className="h-5 w-5" />
                My Predictions
              </CardTitle>
              <CardDescription>
                Click on a prediction to view detailed analysis and object detection results
              </CardDescription>
            </div>
            <div className="flex gap-2">
              <button
                onClick={() => setViewMode("grid")}
                className={`p-2 rounded-md transition-colors ${
                  viewMode === "grid"
                    ? "bg-primary text-primary-foreground"
                    : "bg-muted hover:bg-muted/80"
                }`}
              >
                <Grid className="h-4 w-4" />
              </button>
              <button
                onClick={() => setViewMode("list")}
                className={`p-2 rounded-md transition-colors ${
                  viewMode === "list"
                    ? "bg-primary text-primary-foreground"
                    : "bg-muted hover:bg-muted/80"
                }`}
              >
                <List className="h-4 w-4" />
              </button>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          {predictionsWithDetails.length === 0 ? (
            <div className="text-center py-12">
              <Camera className="h-12 w-12 text-muted-foreground mx-auto mb-4" />
              <h3 className="text-lg font-semibold mb-2">No predictions yet</h3>
              <p className="text-muted-foreground">
                Upload your first image to start detecting objects!
              </p>
            </div>
          ) : viewMode === "grid" ? (
            <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
              {predictionsWithDetails.map((prediction) => (
                <Card 
                  key={prediction.id} 
                  className="overflow-hidden cursor-pointer hover:shadow-lg transition-all duration-200"
                  onClick={() => handlePredictionClick(prediction)}
                >
                  <div className="relative">
                    <img
                      src={prediction.processedImageUrl}
                      alt="Prediction result"
                      className="w-full h-48 object-cover"
                    />
                    <div className="absolute top-2 right-2 bg-black/70 text-white text-xs px-2 py-1 rounded">
                      {prediction.detectionCount} objects
                    </div>
                  </div>
                  <CardContent className="p-4">
                    <div className="flex justify-between items-start mb-2">
                    
                      <span className="text-xs text-muted-foreground">
                        {new Date(prediction.date).toLocaleDateString()}
                      </span>
                    </div>
                    <div className="flex justify-between items-center text-sm">
                      <span className="text-muted-foreground">
                        {prediction.detectionCount} detections
                      </span>
                      {prediction.topCategory && (
                        <span className="bg-primary/10 text-primary px-2 py-1 rounded-full text-xs">
                          {prediction.topCategory}
                        </span>
                      )}
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>
          ) : (
            <div className="space-y-4">
              {predictionsWithDetails.map((prediction) => (
                <Card 
                  key={prediction.id} 
                  className="overflow-hidden cursor-pointer hover:shadow-md transition-all duration-200"
                  onClick={() => handlePredictionClick(prediction)}
                >
                  <CardContent className="p-0">
                    <div className="flex gap-4 p-4">
                      <img
                        src={prediction.processedImageUrl}
                        alt="Prediction result"
                        className="w-20 h-20 object-cover rounded"
                      />
                      <div className="flex-1">
                        <div className="flex justify-between items-start mb-2">
                          
                          <Maximize2 className="h-4 w-4 text-muted-foreground" />
                        </div>
                        <div className="flex justify-between items-center text-sm text-muted-foreground">
                          <span>{new Date(prediction.date).toLocaleString()}</span>
                          <div className="flex gap-4">
                            <span>{prediction.detectionCount} objects detected</span>
                            {prediction.topCategory && (
                              <span className="bg-primary/10 text-primary px-2 py-1 rounded-full text-xs">
                                Top: {prediction.topCategory}
                              </span>
                            )}
                          </div>
                        </div>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
