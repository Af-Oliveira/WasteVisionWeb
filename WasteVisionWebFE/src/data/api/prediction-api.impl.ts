import { ApiError, HttpClient } from "../http/httpClient";
import { IPredictionApi } from "./interfaces/prediction-api";
import {
  PredictionDTO,
  PredictionSearchParamsDTO,
} from "../dto/prediction-dto";


export class PredictionApi implements IPredictionApi {
  constructor(private http: HttpClient) {}

  async getPredictions(params?: PredictionSearchParamsDTO): Promise<PredictionDTO[]> {
    console.log(params?.dateFrom);
    console.log(params?.dateTo);
    console.log(params?.modelName);
    console.log(params?.userName);
    try {
      return await this.http.get<PredictionDTO[]>("/prediction", { params: params || {} });
    } catch (err) {
      if (err instanceof ApiError) throw err;
      throw new ApiError("Failed to fetch predictions", 500);
    }
  }


  async uploadAndDetect(file: File, modelId: string): Promise<PredictionDTO> {
    try {
      const formData = new FormData();
      formData.append("file", file);
      formData.append("roboflowModelId", modelId);

      return await this.http.post<PredictionDTO>("/prediction/upload", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });
    } catch (err) {
      if (err instanceof ApiError) throw err;
      throw new ApiError("Failed to upload and detect image", 500);
    }
  }

}