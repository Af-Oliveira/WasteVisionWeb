import { ObjectPrediction } from "@/core/domain/ObjectPrediction";
import {
  LogDTO,
} from "../dto/log-dto";
import { Log } from "@/core/domain/log";

export class LogMapper {
  /**
   * Maps an LogDTO to a domain Log
   */
  public static toDomain(dto: LogDTO): Log {
    return {
      type: dto.type,
      timestamp: dto.timestamp,
      description: dto.description,
    };
  }

  /**
   * Maps a domain Log to a LogDTO
   */
  public static toDTO(domain: Log): LogDTO {
    return {
      type: domain.type,
      timestamp: domain.timestamp,
      description: domain.description,
    };
  }

  /**
   * Maps an array of LogDTOs to domain Logs
   */
  public static toDomainList(dtos: LogDTO[]): Log[] {
    return dtos.map((dto) => this.toDomain(dto));
  }

  /**
   * Maps an array of domain Logs to LogDTOs
   */
  public static toDTOList(domains: Log[]): LogDTO[] {
    return domains.map((domain) => this.toDTO(domain));
  }

}