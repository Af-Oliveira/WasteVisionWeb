import { z } from "zod";


export const RoboflowModelSchema = z.object({
  id: z.string().optional(),
  description: z.string().min(2).max(100),
  apiKey: z.string().min(2).max(100),
  modelUrl: z.string().min(2).max(100),
  localModelPath: z.string().min(2).max(100),
  endPoint: z.string().url(),
  map: z.string().min(2).max(100),
  recall: z.string().min(2).max(100),
  precision: z.string().min(2).max(100),
  active: z.boolean(),
});

export type RoboflowModel = z.infer<typeof RoboflowModelSchema>;

