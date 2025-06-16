import { Role } from "@/core/domain/Role";
import {
  CreateRoleDTO,
  RoleSearchParamsDTO,
  UpdateRoleDTO,
} from "@/data/dto/role-dto";

export interface IRoleService {
  getRoles(params?: RoleSearchParamsDTO): Promise<Role[]>;
  postRole(role: Role): Promise<Role>;
  putRole(roleId: string, role: Role): Promise<Role>;
  deactivateRole(roleId: string): Promise<boolean>;
  activateRole(roleId: string): Promise<boolean>;
  deleteRole(roleId: string): Promise<boolean>;
}
