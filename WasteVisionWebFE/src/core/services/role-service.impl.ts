import { IRoleApi } from "@/data/api/interfaces/role-api";
import { IRoleService } from "./interfaces/role-service";
import { Role } from "@/core/domain/Role";
import { ApiError } from "@/data/http/httpClient";
import { RoleSearchParamsDTO } from "@/data/dto/role-dto";
import { RoleMapper } from "@/data/mappers/role-mapper";

// More abstraction? Hell yeah.
// If you’re still following, welcome to the corporate multiverse.
export class RoleService implements IRoleService {
  constructor(private roleApi: IRoleApi) {}

  async getRoles(params?: RoleSearchParamsDTO): Promise<Role[]> {
    try {
      const res = await this.roleApi.getRoles(params);
      return RoleMapper.toDomainList(res);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to fetch roles", 500);
    }
  }

  async postRole(role: Role): Promise<Role> {
    try {
      const dto = RoleMapper.toCreateDTO(role);
      const res = await this.roleApi.createRole(dto);
      return RoleMapper.toDomain(res);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to create role", 500);
    }
  }

  async putRole(roleId: string, role: Role): Promise<Role> {
    try {
      const dto = RoleMapper.toUpdateDTO(role);
      const res = await this.roleApi.updateRole(roleId, dto);
      return RoleMapper.toDomain(res);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to update role", 500);
    }
  }

  async deactivateRole(roleId: string): Promise<boolean> {
    try {
      return await this.roleApi.deactivateRole(roleId);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to deactivate role", 500);
    }
  }

  async deleteRole(roleId: string): Promise<boolean> {
    try {
      return await this.roleApi.deleteRole(roleId);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to delete role", 500);
    }
  }

  async activateRole(roleId: string): Promise<boolean> {
    try {
      return await this.roleApi.activateRole(roleId);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to activate role", 500);
    }
  }
}
