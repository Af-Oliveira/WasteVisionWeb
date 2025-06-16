import { IUserApi } from "./interfaces/user-api";
import { ApiError, HttpClient } from "../http/httpClient";
import {
  CreateUserDTO,
  UserDTO,
  UserSearchParamsDTO,
  UpdateUserDTO,
} from "../dto/user-dto";

export class UserApi implements IUserApi {
  constructor(private http: HttpClient) {}

  async getUsers(params?: UserSearchParamsDTO): Promise<UserDTO[]> {
    try {
      return await this.http.get<UserDTO[]>("/User", { params: params || {} });
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to fetch users", 500);
    }
  }

  async createUser(user: CreateUserDTO): Promise<UserDTO> {
    try {
      return await this.http.post<UserDTO>("/User", user);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to create user", 500);
    }
  }

  async updateUser(userId: string, user: UpdateUserDTO): Promise<UserDTO> {
    try {
      return await this.http.put<UserDTO>(`/User/${userId}`, user);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to update user", 500);
    }
  }

  async deactivateUser(userId: string): Promise<boolean> {
    try {
      return await this.http.delete<boolean>(`/User/soft/${userId}`);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to deactivate user", 500);
    }
  }

  async deleteUser(userId: string): Promise<boolean> {
    try {
      return await this.http.delete<boolean>(`/User/hard/${userId}`);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to delete user", 500);
    }
  }

  async activateUser(userId: string): Promise<boolean> {
    try {
      return await this.http.patch<boolean>(`/User/activate/${userId}`);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to activate user", 500);
    }
  }
}
