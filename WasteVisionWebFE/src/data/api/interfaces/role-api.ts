import {
  CreateRoleDTO,
  RoleDTO,
  RoleSearchParamsDTO,
  UpdateRoleDTO,
} from "@/data/dto/role-dto";

export interface IRoleApi {
  getRoles(params?: RoleSearchParamsDTO): Promise<RoleDTO[]>;
  createRole(role: CreateRoleDTO): Promise<RoleDTO>;
  updateRole(roleId: string, role: UpdateRoleDTO): Promise<RoleDTO>;
  deactivateRole(roleId: string): Promise<boolean>;
  activateRole(roleId: string): Promise<boolean>;
  deleteRole(roleId: string): Promise<boolean>;
}
