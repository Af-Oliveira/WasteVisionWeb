// src/hooks/usePredictionSearchForm.ts
import { PredictionSearchParamsDTO } from "@/data/dto/prediction-dto";
import { useState } from "react";
import { date } from "zod";

export function usePredictionSearchForm() {
  const [modelName, setModelName] = useState("");
  const [userName, setUserName] = useState("");
  const [dateFrom, setDateFrom] = useState<Date>();
  const [dateTo, setDateTo] = useState<Date>();
  // Add other searchable fields here if needed, e.g., userId, date range

  const reset = () => {
    setModelName("");
    setUserName("");
    setDateFrom(undefined);
    setDateTo(undefined);
  };

  const getSearchParams = (): PredictionSearchParamsDTO => {
    const params: PredictionSearchParamsDTO = {
    modelName : modelName || undefined,
    userName: userName || undefined,
    dateFrom: dateFrom?.toISOString() || undefined,
    dateTo:  dateTo?.toISOString() || undefined
  };
    return params;
  };

  return {
    formData: {
      modelName,
      userName,
      dateFrom,
      dateTo

    },
    setters: {
      setModelName,
      setUserName,
      setDateFrom,
      setDateTo
    },
    reset,
    getSearchParams,
  };
}
