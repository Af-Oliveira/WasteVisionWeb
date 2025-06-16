import { IPredictionApi } from "@/data/api/interfaces/prediction-api";
import { IPredictionService } from "./interfaces/prediction-service";
import { Prediction } from "@/core/domain/Prediction";
import { ApiError } from "@/data/http/httpClient";
import { PredictionSearchParamsDTO } from "@/data/dto/prediction-dto";
import { PredictionMapper } from "@/data/mappers/prediction-mapper";

export class PredictionService implements IPredictionService {
  constructor(private PredictionApi: IPredictionApi) {}
  postPrediction(prediction: Prediction): Promise<Prediction> {
    throw new Error("Method not implemented.");
  }

  async uploadAndDetect(file: File, modelId: string): Promise<Prediction> {
    try {
      const res = await this.PredictionApi.uploadAndDetect(file,modelId);
      return PredictionMapper.toDomain(res);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to detect trash", 500);
    }
  }

  async getPredictions(params?: PredictionSearchParamsDTO): Promise<Prediction[]> {
    try {
      const res = await this.PredictionApi.getPredictions(params);
      return PredictionMapper.toDomainList(res);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to fetch predictions", 500);
    }
  }



 
}

