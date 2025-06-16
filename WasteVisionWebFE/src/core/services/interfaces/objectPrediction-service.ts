import { ObjectPrediction } from "@/core/domain/ObjectPrediction";
import { ObjectPredictionSearchParamsDTO } from "@/data/dto/objectPrediction-dto";

export interface IObjectPredictionService {
  getObjectPredictions(params?: ObjectPredictionSearchParamsDTO): Promise<ObjectPrediction[]>;

}

