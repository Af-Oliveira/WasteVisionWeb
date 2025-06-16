import {
  CreateRoboflowModelDTO,
  RoboflowModelDTO,
  RoboflowModelSearchParamsDTO,
  UpdateRoboflowModelDTO,
} from "@/data/dto/roboflowModel-dto";

export interface IRoboflowModelApi {
  getRoboflowModels(params?: RoboflowModelSearchParamsDTO): Promise<RoboflowModelDTO[]>;
  getAllActiveRoboflowModels(): Promise<RoboflowModelDTO[]>;
  createRoboflowModel(model: CreateRoboflowModelDTO): Promise<RoboflowModelDTO>;
  updateRoboflowModel(modelId: string, model: UpdateRoboflowModelDTO): Promise<RoboflowModelDTO>;
  deactivateRoboflowModel(modelId: string): Promise<boolean>;
  activateRoboflowModel(modelId: string): Promise<boolean>;
  deleteRoboflowModel(modelId: string): Promise<boolean>;
}