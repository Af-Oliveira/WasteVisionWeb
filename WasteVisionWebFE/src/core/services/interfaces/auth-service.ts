import { LogoutResponse, UserInfo } from "@/core/domain/Auth";

export interface IAuthService {
  login(redirectUrl?: string): void;
  logout(): Promise<LogoutResponse>;
  getUserInfo(): Promise<UserInfo>;
 
}
