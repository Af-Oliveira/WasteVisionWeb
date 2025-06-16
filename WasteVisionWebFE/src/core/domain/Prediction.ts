import {  z } from "zod";
import { RoboflowModelSchema } from "./RoboflowModel";
import { UserSchema } from "./User";


export const PredictionSchema = z.object({
  id: z.string().optional(),
  userId: z.string(),
  modelId: z.string(),
  date: z.string(),
  originalImageUrl: z.string().url(),
  processedImageUrl: z.string().url(),
  roboflowModel: RoboflowModelSchema.optional(),
  user: UserSchema.optional(),
});

export type Prediction = z.infer<typeof PredictionSchema>;