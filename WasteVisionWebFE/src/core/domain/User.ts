import { z } from "zod";
import { RoleSchema } from "./Role";

export const UserSchema = z.object({
  id: z.string().optional(),
  email: z.string().email(),
  username: z.string().min(3),
  active: z.boolean().default(true),
  role: RoleSchema.optional(),
});

export type User = z.infer<typeof UserSchema>;
