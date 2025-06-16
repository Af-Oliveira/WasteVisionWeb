import {
  PredictionDTO,
  PredictionSearchParamsDTO,

} from "@/data/dto/prediction-dto";

export interface IPredictionApi {
  getPredictions(params?: PredictionSearchParamsDTO): Promise<PredictionDTO[]>;
  uploadAndDetect(file: File, modelId: string): Promise<PredictionDTO>;
}