import { Role } from "@/core/domain/Role";

export interface UserDTO {
  id?: string;
  email: string;
  username: string;
  roleId: string;
  roleName?: string;
  active: boolean;
  role?: Role;
}

export interface CreateUserDTO {
  email: string;
  username: string;
  roleId: string;
}

export interface UpdateUserDTO {
  email: string;
  username: string;
  roleId: string;
}

export interface UserSearchParamsDTO {
  email?: string;
  username?: string;
  roleId?: string;
}
