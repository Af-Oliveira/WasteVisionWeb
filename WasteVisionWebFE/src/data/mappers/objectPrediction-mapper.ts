import { ObjectPrediction } from "@/core/domain/ObjectPrediction";
import {
  ObjectPredictionDTO,
} from "../dto/objectPrediction-dto";

export class ObjectPredictionMapper {
  /**
   * Maps an ObjectPredictionDTO to a domain ObjectPrediction
   */
  public static toDomain(dto: ObjectPredictionDTO): ObjectPrediction {
    return {
      id: dto.id,
      x: dto.x,
      y: dto.y,
      width: dto.width,
      height: dto.height,
      category: dto.category,
      confidence: dto.confidence,
      predictionId: dto.predictionId,
    };
  }

  /**
   * Maps a domain ObjectPrediction to an ObjectPredictionDTO
   */
  public static toDTO(domain: ObjectPrediction): ObjectPredictionDTO {
    return {
      id: domain.id,
      x: domain.x,
      y: domain.y,
      width: domain.width,
      height: domain.height,
      category: domain.category,
      confidence: domain.confidence,
      predictionId: domain.predictionId,

    };
  }

  /**
   * Maps an array of ObjectPredictionDTOs to domain ObjectPredictions
   */
  public static toDomainList(dtos: ObjectPredictionDTO[]): ObjectPrediction[] {
    return dtos.map((dto) => this.toDomain(dto));
  }

  /**
   * Maps an array of domain ObjectPredictions to ObjectPredictionDTOs
   */
  public static toDTOList(domains: ObjectPrediction[]): ObjectPredictionDTO[] {
    return domains.map((domain) => this.toDTO(domain));
  }

}