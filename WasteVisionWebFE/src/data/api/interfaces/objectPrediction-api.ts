import {
  ObjectPredictionDTO,
  ObjectPredictionSearchParamsDTO,

} from "@/data/dto/objectPrediction-dto";

export interface IObjectPredictionApi {
  getObjectPredictions(params?: ObjectPredictionSearchParamsDTO): Promise<ObjectPredictionDTO[]>;
  
}