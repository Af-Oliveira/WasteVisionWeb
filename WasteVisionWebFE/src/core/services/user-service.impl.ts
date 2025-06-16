import { IUserApi } from "@/data/api/interfaces/user-api";
import { IUserService } from "./interfaces/user-service";
import { User } from "@/core/domain/User";
import { ApiError } from "@/data/http/httpClient";
import { UserSearchParamsDTO } from "@/data/dto/user-dto";
import { UserMapper } from "@/data/mappers/user-mapper";

export class UserService implements IUserService {
  constructor(private userApi: IUserApi) {}

  async getUsers(params?: UserSearchParamsDTO): Promise<User[]> {
    try {
      const res = await this.userApi.getUsers(params);
      return UserMapper.toDomainList(res);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to fetch users", 500);
    }
  }

  async postUser(user: User): Promise<User> {
    try {
      const dto = UserMapper.toCreateDTO(user);
      const res = await this.userApi.createUser(dto);
      return UserMapper.toDomain(res);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to create user", 500);
    }
  }

  async putUser(userId: string, user: User): Promise<User> {
    try {
      const dto = UserMapper.toUpdateDTO(user);
      const res = await this.userApi.updateUser(userId, dto);
      return UserMapper.toDomain(res);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to update user", 500);
    }
  }

  async deactivateUser(userId: string): Promise<boolean> {
    try {
      return await this.userApi.deactivateUser(userId);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to deactivate user", 500);
    }
  }

  async deleteUser(userId: string): Promise<boolean> {
    try {
      return await this.userApi.deleteUser(userId);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to delete user", 500);
    }
  }

  async activateUser(userId: string): Promise<boolean> {
    try {
      return await this.userApi.activateUser(userId);
    } catch (err) {
      if (err instanceof ApiError) {
        throw err;
      }
      throw new ApiError("Failed to activate user", 500);
    }
  }
}
