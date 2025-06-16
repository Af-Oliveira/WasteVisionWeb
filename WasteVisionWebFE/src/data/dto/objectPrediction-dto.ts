

export interface ObjectPredictionDTO {
  id?: string;
  x: string;
  y: string;
  width: string;
  height: string;
  category: string;
  confidence: string;
  predictionId: string;
}


export interface ObjectPredictionSearchParamsDTO {
  predictionId: string;
  precisionFrom?: string;
  precisionTo?: string;
}
