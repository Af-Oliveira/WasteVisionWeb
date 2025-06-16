import { User } from "@/core/domain/User";
import {
  UserDTO,
  CreateUserDTO,
  UpdateUserDTO,
  UserSearchParamsDTO,
} from "../dto/user-dto";

export class UserMapper {
  /**
   * Maps a UserDTO to a domain User
   */
  public static toDomain(dto: UserDTO): User {
    return {
      id: dto.id,
      email: dto.email,
      username: dto.username,
      active: dto.active,
      role: {
        id: dto.roleId,
        description: dto.roleName || "",
        active: true,
      },
    };
  }

  /**
   * Maps a domain User to a UserDTO
   */
  public static toDTO(domain: User): UserDTO {
    return {
      id: domain.id,
      email: domain.email,
      roleId: domain.role?.id || "",
      username: domain.username,
      active: domain.active,
      role: domain.role,
    };
  }

  /**
   * Maps an array of UserDTOs to domain Users
   */
  public static toDomainList(dtos: UserDTO[]): User[] {
    return dtos.map((dto) => this.toDomain(dto));
  }

  /**
   * Maps a domain User to a CreateUserDTO
   */
  public static toCreateDTO(domain: User): CreateUserDTO {
    return {
      email: domain.email,
      username: domain.username,
      roleId: domain.role?.id || "",
    };
  }

  /**
   * Maps a domain User to an UpdateUserDTO
   */
  public static toUpdateDTO(domain: User): UpdateUserDTO {
    return {
      email: domain.email,
      username: domain.username,
      roleId: domain.role?.id || "",
    };
  }
}
