import {
  LogDTO,
} from "@/data/dto/log-dto";

export interface ILogApi {
  getLogs(): Promise<LogDTO[]>;
}