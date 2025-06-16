import { IRoleApi } from "./interfaces/role-api";
import { ApiError, HttpClient } from "../http/httpClient";
import {
  CreateRoleDTO,
  RoleDTO,
  RoleSearchParamsDTO,
  UpdateRoleDTO,
} from "../dto/role-dto";

export class RoleApi implements IRoleApi {
  constructor(private http: HttpClient) {}

  async getRoles(params?: RoleSearchParamsDTO): Promise<RoleDTO[]> {
    try {
      return await this.http.get<RoleDTO[]>("/role", { params: params || {} });
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to fetch roles", 500);
    }
  }

  async createRole(role: CreateRoleDTO): Promise<RoleDTO> {
    try {
      return await this.http.post<RoleDTO>("/role", role);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to create role", 500);
    }
  }

  async updateRole(roleId: string, role: UpdateRoleDTO): Promise<RoleDTO> {
    try {
      return await this.http.put<RoleDTO>(`/Role/${roleId}`, role);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to update role", 500);
    }
  }

  async deactivateRole(roleId: string): Promise<boolean> {
    try {
      return await this.http.delete<boolean>(`/Role/soft/${roleId}`);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to deactivate role", 500);
    }
  }

  async deleteRole(roleId: string): Promise<boolean> {
    try {
      return await this.http.delete<boolean>(`/Role/hard/${roleId}`);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to delete role", 500);
    }
  }

  async activateRole(roleId: string): Promise<boolean> {
    try {
      return await this.http.patch<boolean>(`/Role/activate/${roleId}`);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to activate role", 500);
    }
  }
}
