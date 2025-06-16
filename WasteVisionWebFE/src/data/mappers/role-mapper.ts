import { Role } from "@/core/domain/Role";
import {
  RoleDTO,
  CreateRoleDTO,
  UpdateRoleDTO,
  RoleSearchParamsDTO,
} from "@/data/dto/role-dto";

export class RoleMapper {
  /**
   * Maps a RoleDTO to a domain Role
   */
  public static toDomain(dto: RoleDTO): Role {
    return {
      id: dto.id,
      description: dto.description,
      active: dto.active,
    };
  }

  /**
   * Maps a domain Role to a RoleDTO
   */
  public static toDTO(domain: Role): RoleDTO {
    return {
      id: domain.id,
      description: domain.description,
      active: domain.active,
    };
  }

  /**
   * Maps an array of RoleDTOs to domain Roles
   */
  public static toDomainList(dtos: RoleDTO[]): Role[] {
    return dtos.map((dto) => this.toDomain(dto));
  }

  /**
   * Maps a domain Role to a CreateRoleDTO
   */
  public static toCreateDTO(domain: Role): CreateRoleDTO {
    return {
      description: domain.description,
    };
  }

  /**
   * Maps a domain Role to an UpdateRoleDTO
   */
  public static toUpdateDTO(domain: Role): UpdateRoleDTO {
    return {
      description: domain.description,
    };
  }

  /**
   * Maps search parameters to a RoleSearchParamsDTO
   */
  public static toSearchParamsDTO(description?: string): RoleSearchParamsDTO {
    return {
      description,
    };
  }
}
