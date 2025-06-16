
import { ApiError } from "@/data/http/httpClient";
import { Log } from "../domain/log";
import { ILogService } from "./interfaces/log-service";
import { ILogApi } from "@/data/api/interfaces/log-api";
import { LogMapper } from "@/data/mappers/log-mapper";

export class LogService implements ILogService {
  constructor(private LogApi: ILogApi) {}

  async getLogs(): Promise<Log[]> {
    try {
      const res = await this.LogApi.getLogs();
      return LogMapper.toDomainList(res);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to fetch logs", 500);
    }
  }

  
}

