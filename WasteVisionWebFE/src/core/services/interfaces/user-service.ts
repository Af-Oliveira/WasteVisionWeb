import { User } from "@/core/domain/User";
import { UserSearchParamsDTO } from "@/data/dto/user-dto";

export interface IUserService {
  getUsers(params?: UserSearchParamsDTO): Promise<User[]>;
  postUser(user: User): Promise<User>;
  putUser(userId: string, user: User): Promise<User>;
  deactivateUser(userId: string): Promise<boolean>;
  activateUser(userId: string): Promise<boolean>;
  deleteUser(userId: string): Promise<boolean>;
}
