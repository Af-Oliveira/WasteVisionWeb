import { ApiError, HttpClient } from "../http/httpClient";
import { IObjectPredictionApi } from "./interfaces/objectPrediction-api";
import {
  ObjectPredictionDTO,
  ObjectPredictionSearchParamsDTO,
} from "../dto/objectPrediction-dto";
import { Console } from "console";


export class ObjectPredictionApi implements IObjectPredictionApi {
  constructor(private http: HttpClient) {}


  async getObjectPredictions(params?: ObjectPredictionSearchParamsDTO): Promise<ObjectPredictionDTO[]> {
    try {

      return await this.http.get<ObjectPredictionDTO[]>("/objectprediction", { params: params || {} });
    } catch (err) {
      if (err instanceof ApiError) throw err;
      throw new ApiError("Failed to fetch ObjectPredictions", 500);
    }
  }

}