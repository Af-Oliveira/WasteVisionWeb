import { config } from "@/config";
import { ApiError, HttpClient } from "../http/httpClient";
import {  UserInfoDTO } from "../dto/auth-dto";
import { IAuthApi } from "./interfaces/auth-api";

export class AuthApi implements IAuthApi {
  constructor(private http: HttpClient) {}

  login(redirectUrl: string): void {
    const clientRootURL = encodeURIComponent(redirectUrl);
    const loginUrl = `${config.API_URL}/auth/login?clientRootURL=${clientRootURL}`;
    window.location.href = loginUrl;
  }

  async logout(): Promise<string> {
    try {
      const res = await this.http.get<string>("/auth/logout");
      return res;
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to logout", 500);
    }
  }

  async getUserInfo(): Promise<UserInfoDTO> {
    try {
      return await this.http.get<UserInfoDTO>("/auth/me");
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to fetch user info", 500);
    }
  }

}
