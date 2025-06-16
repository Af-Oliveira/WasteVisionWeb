import { Prediction } from "@/core/domain/Prediction";
import { PredictionDTO } from "../dto/prediction-dto";

export class PredictionMapper {
  /**
   * Maps a PredictionDTO to a domain Prediction
   */
  public static toDomain(dto: PredictionDTO): Prediction {
    return {
      id: dto.id,
      userId: dto.userId,
      modelId: dto.modelId,
      originalImageUrl: dto.originalImageUrl,
      processedImageUrl: dto.processedImageUrl,
      roboflowModel: {
        id: dto.modelId,
        description: dto.modelName || "",
        apiKey: "",
        modelUrl: "",
        localModelPath: "",
        endPoint: "",
        map: "",
        recall: "",
        precision: "",
        active: true,
      },
      user:{
        id: dto.userId,
        username: dto.userName || "",
        email: "",
        active: true,
      },
      date: dto.date,
    };
  }


  /**
   * Maps a domain Prediction to a PredictionDTO
   */
  public static toDTO(domain: Prediction): PredictionDTO {
    return {
      id: domain.id,
      userId: domain.userId,
      modelId: domain.modelId,
      originalImageUrl: domain.originalImageUrl,
      processedImageUrl: domain.processedImageUrl,
      date: domain.date,
      roboflowModel: domain.roboflowModel,
      user: domain.user,
    };
  }

  /**
   * Maps an array of PredictionDTOs to domain Predictions
   */
  public static toDomainList(dtos: PredictionDTO[]): Prediction[] {
    return dtos.map((dto) => this.toDomain(dto));
  }


}