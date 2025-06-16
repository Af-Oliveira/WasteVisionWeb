import {  z } from "zod";


export const LogSchema = z.object({
  type: z.string(),
  timestamp: z.string(),
  description: z.string(),
});

export type Log = z.infer<typeof LogSchema>;