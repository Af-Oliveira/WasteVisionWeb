// src/features/predictions/pages/PredictionsPage.tsx
import { useCallback, useEffect, useState } from "react";
import { Prediction } from "@/core/domain/Prediction";
import { IPredictionService } from "@/core/services/interfaces/prediction-service";
import { PredictionSearchParamsDTO } from "@/data/dto/prediction-dto";
import { ApiError } from "@/data/http/httpClient";
import { usePredictionService } from "@/di/container"; // Ensure this hook is set up in your DI
import { useToast } from "@/hooks/useToast";
import { usePredictionSearchForm } from "@/hooks/usePredictionForm";
import { TableHeader } from "../common/TableHeader.tsx";
import { DataTable } from "./data-table.tsx";
import { columns } from "./columns";
import { PredictionSearchForm } from "./components/PredictionSearchForm";

export function PredictionsPage() {
  const [loading, setLoading] = useState(false);
  const [predictions, setPredictions] = useState<Prediction[]>([]);
  const [isSearchFormOpen, setIsSearchFormOpen] = useState(false);

  const predictionService: IPredictionService = usePredictionService();
  const { success, error } = useToast();

  const {
    formData: searchFormData,
    setters: searchSetters,
    reset: resetSearch,
    getSearchParams,
  } = usePredictionSearchForm();

  const fetchPredictions = useCallback(
    async (params?: PredictionSearchParamsDTO) => {
      setLoading(true);
      try {
        const res = await predictionService.getPredictions(params);
        setPredictions(res);
      } catch (err) {
        const apiError =
          err instanceof ApiError
            ? err
            : new ApiError("Failed to fetch predictions", 500);
        error(apiError.message);
      } finally {
        setLoading(false);
      }
    },
    [predictionService, error],
  );

  useEffect(() => {
    fetchPredictions();
  }, []);

  const handleSearchSubmit = async () => {
    const params = getSearchParams();
    await fetchPredictions(params);
    setIsSearchFormOpen(false);
    if (Object.keys(params).length > 0) {
      success("Search applied.");
    } else {
      success("Search cleared.");
    }
  };

  const handleRefresh = () => {
    resetSearch(); // Optionally reset search when refreshing all
    fetchPredictions();
  };

  return (
    <div className="space-y-4">
      <TableHeader
        title="Image Predictions"
        // No onCreate prop as per requirements
        isLoading={loading}
        onRefresh={handleRefresh}
        onSearch={() => setIsSearchFormOpen(true)}
      />

      <PredictionSearchForm
        isOpen={isSearchFormOpen}
        onClose={() => setIsSearchFormOpen(false)}
        onSubmit={handleSearchSubmit}
        formData={searchFormData}
        setters={searchSetters}
        reset={() => {
          resetSearch();
          // Optionally, you might want to immediately fetch all results after reset
          // fetchPredictions();
        }}
      />

      <DataTable columns={columns} data={predictions} />
      {/* No actions prop is passed to DataTable, so no action column will be rendered */}
    </div>
  );
}
