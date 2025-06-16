import { IObjectPredictionApi } from "@/data/api/interfaces/objectPrediction-api";
import { IObjectPredictionService } from "./interfaces/objectPrediction-service";
import { ObjectPrediction } from "@/core/domain/ObjectPrediction";
import { ApiError } from "@/data/http/httpClient";
import { ObjectPredictionSearchParamsDTO } from "@/data/dto/objectPrediction-dto";
import { ObjectPredictionMapper } from "@/data/mappers/objectPrediction-mapper";

export class ObjectPredictionService implements IObjectPredictionService {
  constructor(private ObjectPredictionApi: IObjectPredictionApi) {}

  async getObjectPredictions(params?: ObjectPredictionSearchParamsDTO): Promise<ObjectPrediction[]> {
    try {
      const res = await this.ObjectPredictionApi.getObjectPredictions(params);
      return ObjectPredictionMapper.toDomainList(res);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to fetch ObjectPredictions", 500);
    }
  }

  
}

