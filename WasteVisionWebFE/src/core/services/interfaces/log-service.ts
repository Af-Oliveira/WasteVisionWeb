import { Log } from "@/core/domain/Log";

export interface ILogService {
  getLogs(): Promise<Log[]>;
}
