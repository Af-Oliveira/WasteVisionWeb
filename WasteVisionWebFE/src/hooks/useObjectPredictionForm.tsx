// src/hooks/usePredictionSearchForm.ts
import { ObjectPredictionSearchParamsDTO } from "@/data/dto/objectPrediction-dto";
import { useState } from "react";

export function useObjectPredictionSearchForm() {
  const [precisionFrom, setPrecisionFrom] = useState<number | undefined>();
  const [precisionTo, setPrecisionTo] = useState<number | undefined>();
  // Add other searchable fields here if needed, e.g., userId, date range

  const reset = () => {
    setPrecisionFrom(undefined);
    setPrecisionTo(undefined);
  };

  const getSearchParams = (): ObjectPredictionSearchParamsDTO => {
    const params: ObjectPredictionSearchParamsDTO = {
      precisionFrom: precisionFrom?.toString() || undefined,
      precisionTo: precisionTo?.toString() || undefined
    };
    return params;
  };

  return {
    formData: {
      precisionFrom,
      precisionTo
    },
    setters: {
      setPrecisionFrom,
      setPrecisionTo
    },
    reset,
    getSearchParams,
  };
}