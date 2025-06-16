import { Role } from "@/core/domain/Role";

export interface UserInfoDTO {
  id?: string;
  email?: string;
  username: string;
  role?: Role;
  active?: boolean;
}


export interface AuthResponseDTO {
  token: string;
  user: UserInfoDTO;
}

export interface LogoutResponseDTO {
  message: string;
}
