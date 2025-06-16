


import { RoboflowModel } from "../domain/RoboflowModel";
import { IRoboflowModelService } from "./interfaces/roboflowModel-service";
import { IRoboflowModelApi } from "@/data/api/interfaces/roboflowModel-api";
import { ApiError } from "@/data/http/httpClient";
import { CreateRoboflowModelDTO, RoboflowModelSearchParamsDTO, UpdateRoboflowModelDTO } from "@/data/dto/roboflowModel-dto";
import { RoboflowModelMapper } from "@/data/mappers/roboflowModel-mapper";
import { UpdatePayload } from "vite/types/hmrPayload.js";


export class RoboflowModelService implements IRoboflowModelService {
  constructor(private roboflowModelApi: IRoboflowModelApi) {}

  async getRoboflowModels(params?: RoboflowModelSearchParamsDTO): Promise<RoboflowModel[]> {
      try {
        const res = await this.roboflowModelApi.getRoboflowModels(params);
        return RoboflowModelMapper.toDomainList(res);
      } catch (err) {
        if (err instanceof ApiError) {
          throw err;
        }
        throw new ApiError("Failed to fetch RoboflowModels", 500);
      }
    }

    async getAllActiveRoboflowModels(): Promise<RoboflowModel[]> {
      try {
        const res = await this.roboflowModelApi.getAllActiveRoboflowModels();
        return RoboflowModelMapper.toDomainList(res);
      } catch (err) {
        if (err instanceof ApiError) {
          throw err;
        }
        throw new ApiError("Failed to fetch active RoboflowModels", 500);
      }
    }
  
    async postRoboflowModel(RoboflowModel: CreateRoboflowModelDTO): Promise<RoboflowModel> {
      try {
        const res = await this.roboflowModelApi.createRoboflowModel(RoboflowModel);
        return RoboflowModelMapper.toDomain(res);
      } catch (err) {
        if (err instanceof ApiError) {
          throw err;
        }
        throw new ApiError("Failed to create RoboflowModel", 500);
      }
    }
  
    async putRoboflowModel(RoboflowModelId: string, RoboflowModel: UpdateRoboflowModelDTO): Promise<RoboflowModel> {
      try {
        const res = await this.roboflowModelApi.updateRoboflowModel(RoboflowModelId, RoboflowModel);
        return RoboflowModelMapper.toDomain(res);
      } catch (err) {
        if (err instanceof ApiError) {
          throw err;
        }
        throw new ApiError("Failed to update RoboflowModel", 500);
      }
    }
  
    async deactivateRoboflowModel(RoboflowModelId: string): Promise<boolean> {
      try {
        return await this.roboflowModelApi.deactivateRoboflowModel(RoboflowModelId);
      } catch (err) {
        if (err instanceof ApiError) {
          throw err;
        }
        throw new ApiError("Failed to deactivate RoboflowModel", 500);
      }
    }
  
    async deleteRoboflowModel(RoboflowModelId: string): Promise<boolean> {
      try {
        return await this.roboflowModelApi.deleteRoboflowModel(RoboflowModelId);
      } catch (err) {
        if (err instanceof ApiError) {
          throw err;
        }
        throw new ApiError("Failed to delete RoboflowModel", 500);
      }
    }
  
    async activateRoboflowModel(RoboflowModelId: string): Promise<boolean> {
      try {
        return await this.roboflowModelApi.activateRoboflowModel(RoboflowModelId);
      } catch (err) {
        if (err instanceof ApiError) {
          throw err;
        }
        throw new ApiError("Failed to activate RoboflowModel", 500);
      }
    }

}