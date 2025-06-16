import { UserInfo } from "@/core/domain/Auth";
import { UserInfoDTO } from "../dto/auth-dto";

export class AuthMapper {
  /**
   * Maps a UserInfoDTO to domain UserInfo
   */
  public static toDomain(dto: UserInfoDTO): UserInfo {
    return {
      id: dto.id,
      email: dto.email,
      username: dto.username,
      role: dto.role,
      active: dto.active,
    };
  }

  /**
   * Maps domain UserInfo to UserInfoDTO
   */
  public static toDTO(domain: UserInfo): UserInfoDTO {
    return {
      id: domain.id,
      email: domain.email,
      username: domain.username,
      role: domain.role,
      active: domain.active,
    };
  }


}
