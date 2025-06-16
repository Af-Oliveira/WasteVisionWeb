import { LogoutResponse, UserInfo } from "@/core/domain/Auth";
import { IAuthService } from "./interfaces/auth-service";
import { ApiError } from "@/data/http/httpClient";
import { AuthMapper } from "@/data/mappers/auth-mapper";
import { IAuthApi } from "@/data/api/interfaces/auth-api";

export class AuthService implements IAuthService {
  constructor(private authApi: IAuthApi) {}

  login(redirectUrl: string = window.location.origin + "/login"): void {
    this.authApi.login(redirectUrl);
  }

  async logout(): Promise<LogoutResponse> {
    try {
      const res = await this.authApi.logout();

      // Redirect after successful logout
      const redirectUri = `${window.location.origin}`;
      window.location.href = redirectUri;

      return res;
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to logout", 500);
    }
  }

  async getUserInfo(): Promise<UserInfo> {
    try {
      const userInfoDTO = await this.authApi.getUserInfo();
      return AuthMapper.toDomain(userInfoDTO);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to fetch user info", 500);
    }
  }
}
