import { UserInfoDTO } from "@/data/dto/auth-dto";

export interface IAuthApi {
  login(redirectUrl: string): void;
  logout(): Promise<string>;
  getUserInfo(): Promise<UserInfoDTO>;
}
