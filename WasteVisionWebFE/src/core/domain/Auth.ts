import { z } from "zod";
import { RoleSchema } from "./Role";

export const UserInfoSchema = z.object({
  id: z.string().optional(),
  email: z.string().email().optional(),
  username: z.string().min(3),
  role: RoleSchema.optional(),
  active: z.boolean().optional(),
});

export type UserInfo = z.infer<typeof UserInfoSchema>;
export type AuthToken = string;
export type LogoutResponse = string;
