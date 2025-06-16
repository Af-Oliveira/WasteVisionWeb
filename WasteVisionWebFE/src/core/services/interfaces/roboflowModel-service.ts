import { RoboflowModel } from "@/core/domain/RoboflowModel";
import { CreateRoboflowModelDTO, RoboflowModelSearchParamsDTO, UpdateRoboflowModelDTO } from "@/data/dto/roboflowModel-dto";

export interface IRoboflowModelService {
  getRoboflowModels(params?: RoboflowModelSearchParamsDTO): Promise<RoboflowModel[]>;
  getAllActiveRoboflowModels(): Promise<RoboflowModel[]>;
  postRoboflowModel(model: CreateRoboflowModelDTO): Promise<RoboflowModel>;
  putRoboflowModel(modelId: string, model: UpdateRoboflowModelDTO): Promise<RoboflowModel>;
  deactivateRoboflowModel(modelId: string): Promise<boolean>;
  activateRoboflowModel(modelId: string): Promise<boolean>;
  deleteRoboflowModel(modelId: string): Promise<boolean>;
}