import { Prediction } from "@/core/domain/Prediction";
import { PredictionSearchParamsDTO } from "@/data/dto/prediction-dto";

export interface IPredictionService {
  uploadAndDetect(file: File, modelId: string): Promise<Prediction>;
  getPredictions(params?: PredictionSearchParamsDTO): Promise<Prediction[]>;
}