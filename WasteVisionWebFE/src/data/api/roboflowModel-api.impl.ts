
import { ApiError, HttpClient } from "../http/httpClient";
import { IRoboflowModelApi } from "./interfaces/roboflowModel-api";
import {
  CreateRoboflowModelDTO,
  UpdateRoboflowModelDTO,
  RoboflowModelDTO,
  RoboflowModelSearchParamsDTO,
} from "../dto/roboflowModel-dto";

export class RoboflowModelApi implements IRoboflowModelApi {
  constructor(private http: HttpClient) {}

  async getRoboflowModels(params?: RoboflowModelSearchParamsDTO): Promise<RoboflowModelDTO[]> {
    try {
      
      return await this.http.get<RoboflowModelDTO[]>("/roboflowmodel", { params: params || {} });
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to fetch Roboflow models", 500);
    }
  }

  async getAllActiveRoboflowModels(): Promise<RoboflowModelDTO[]> {
    try {
      return await this.http.get<RoboflowModelDTO[]>("/roboflowmodel/active");
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to fetch active Roboflow models", 500);
    }
  }

  async createRoboflowModel(dto: CreateRoboflowModelDTO): Promise<RoboflowModelDTO> {
    try {
       const formData = new FormData();
      formData.append("description", dto.description);
      formData.append("apiKey", dto.apiKey);
      formData.append("modelUrl", dto.modelUrl);
      formData.append("modelFile", dto.modelFile);


      return await this.http.post<RoboflowModelDTO>("/roboflowmodel", formData, {
              headers: {
                "Content-Type": "multipart/form-data",
              },
            });
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to create Roboflow model", 500);
    }
  }

  async updateRoboflowModel(id: string, dto: UpdateRoboflowModelDTO): Promise<RoboflowModelDTO> {
    try {
       const formData = new FormData();
      formData.append("description", dto.description);
      formData.append("apiKey", dto.apiKey);
      formData.append("modelUrl", dto.modelUrl);
      formData.append("modelFile", dto.modelFile);

      return await this.http.put<RoboflowModelDTO>(`/roboflowmodel/${id}`, formData, {
              headers: {
                "Content-Type": "multipart/form-data",
              },
            });
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to update Roboflow model", 500);
    }
  }

  async deactivateRoboflowModel(id: string): Promise<boolean> {
    try {
      return await this.http.patch<boolean>(`/roboflowmodel/deactivate/${id}`);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to deactivate Roboflow model", 500);
    }
  }

  async activateRoboflowModel(id: string): Promise<boolean> {
    try {
      return await this.http.patch<boolean>(`/roboflowmodel/activate/${id}`);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to activate Roboflow model", 500);
    }
  }

  async deleteRoboflowModel(id: string): Promise<boolean> {
    try {
      return await this.http.delete<boolean>(`/roboflowmodel/${id}`);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to delete Roboflow model", 500);
    }
  }
}