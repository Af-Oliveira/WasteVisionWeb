import { ApiError, HttpClient } from "../http/httpClient";
import { LogDTO } from "../dto/log-dto";
import { ILogApi } from "./interfaces/log-api";


export class LogApi implements ILogApi {
  constructor(private http: HttpClient) {}


  async getLogs(): Promise<LogDTO[]> {
    try {

      return await this.http.get<LogDTO[]>("/logs");
    } catch (err) {
      if (err instanceof ApiError) throw err;
      throw new ApiError("Failed to fetch logs", 500);
    }
  }

}