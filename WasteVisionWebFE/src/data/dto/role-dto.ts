export interface RoleDTO {
  id?: string;
  description: string;
  active: boolean;
}

export interface CreateRoleDTO {
  description: string;
}

export interface UpdateRoleDTO {
  description: string;
}

export interface RoleSearchParamsDTO {
  description?: string;
}
