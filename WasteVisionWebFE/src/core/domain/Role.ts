import { z } from "zod";

export const RoleSchema = z.object({
  id: z.string().optional(),
  description: z.string().min(3).max(100),
  active: z.boolean().default(true),
});

export type Role = z.infer<typeof RoleSchema>;
