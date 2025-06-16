import { RoboflowModel } from "@/core/domain/RoboflowModel";
import { User } from "@/core/domain/User";




export interface PredictionDTO {
  id?: string;
  userId: string;
  modelId: string;
  modelName?: string;
  userName?: string;
  originalImageUrl: string;
  processedImageUrl: string;
  date: string;
  user?: User;
  roboflowModel?: RoboflowModel 
}

export interface PredictionSearchParamsDTO {
  userId?: string;
  modelName?: string;
  userName?: string;
  dateFrom?: string;
  dateTo?: string;
}

