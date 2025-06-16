import { z } from "zod";

export const ObjectPredictionSchema = z.object({
  id: z.string().optional(),
  confidence: z.string().min(0).max(100),
  category: z.string().min(2).max(100),
  x: z.string().min(0),
  y: z.string().min(0),
  width: z.string().min(0),
  height: z.string().min(0),
  predictionId: z.string(),
});

export type ObjectPrediction = z.infer<typeof ObjectPredictionSchema>;