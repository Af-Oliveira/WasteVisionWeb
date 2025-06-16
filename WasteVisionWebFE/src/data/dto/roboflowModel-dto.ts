import { Role } from "@/core/domain/Role";

export interface RoboflowModelDTO {
  id?: string;
  description: string;
  apiKey: string;
  modelUrl: string;
  localModelPath: string;
  endPoint: string;
  map: string;
  recall: string;
  precision: string;
  active: boolean;
  
}

export interface CreateRoboflowModelDTO {
  description: string;
  apiKey: string;
  modelUrl: string;
  modelFile: File;
}

export interface UpdateRoboflowModelDTO {
  description: string;
  apiKey: string;
  modelUrl: string;
  modelFile: File;
}

export interface RoboflowModelSearchParamsDTO {
   description?: string;
   active?: string;
   modelUrl?: string;
 
}

