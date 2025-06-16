import { RoboflowModel } from "@/core/domain/RoboflowModel";
import {
  RoboflowModelDTO,
  CreateRoboflowModelDTO,
  UpdateRoboflowModelDTO,
} from "../dto/roboflowModel-dto";

export class RoboflowModelMapper {
  /**
   * Maps a RoboflowModelDTO to a domain RoboflowModel
   */
  public static toDomain(dto: RoboflowModelDTO): RoboflowModel {
    return {
      id: dto.id,
      description: dto.description,
      apiKey: dto.apiKey,
      modelUrl: dto.modelUrl,
      localModelPath: dto.localModelPath,
      endPoint: dto.endPoint,
      map: dto.map,
      recall: dto.recall,
      precision: dto.precision,
      active: dto.active,
    };
  }

  /**
   * Maps a domain RoboflowModel to a RoboflowModelDTO
   */
  public static toDTO(domain: RoboflowModel): RoboflowModelDTO {
    return {
      id: domain.id,
      description: domain.description,
      apiKey: domain.apiKey,
      modelUrl: domain.modelUrl,
      localModelPath: domain.localModelPath,
      endPoint: domain.endPoint,
      map: domain.map,
      recall: domain.recall,
      precision: domain.precision,
      active: domain.active,
    };
  }

  /**
   * Maps an array of RoboflowModelDTOs to domain RoboflowModels
   */
  public static toDomainList(dtos: RoboflowModelDTO[]): RoboflowModel[] {
    return dtos.map((dto) => this.toDomain(dto));
  }


}