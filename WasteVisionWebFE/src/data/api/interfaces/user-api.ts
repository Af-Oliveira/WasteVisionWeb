import {
  CreateUserDTO,
  UserDTO,
  UserSearchParamsDTO,
  UpdateUserDTO,
} from "@/data/dto/user-dto";

export interface IUserApi {
  getUsers(params?: UserSearchParamsDTO): Promise<UserDTO[]>;
  createUser(user: CreateUserDTO): Promise<UserDTO>;
  updateUser(userId: string, user: UpdateUserDTO): Promise<UserDTO>;
  deactivateUser(userId: string): Promise<boolean>;
  activateUser(userId: string): Promise<boolean>;
  deleteUser(userId: string): Promise<boolean>;
}
