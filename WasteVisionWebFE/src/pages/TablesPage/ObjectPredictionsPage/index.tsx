// src/features/predictions/pages/PredictionsPage.tsx
import { useCallback, useEffect, useState } from "react";
import { ApiError } from "@/data/http/httpClient";
import { useObjectPredictionService } from "@/di/container"; // Ensure this hook is set up in your DI
import { useToast } from "@/hooks/useToast";
import { useObjectPredictionSearchForm } from "@/hooks/useObjectPredictionForm.tsx";
import { TableHeader } from "../common/TableHeader.tsx";
import { DataTable } from "./data-table.tsx";
import { columns } from "./columns.tsx";
import { ObjectPredictionSearchForm } from "./components/ObjectPredictionSearchForm.tsx";
import { IObjectPredictionService } from "@/core/services/interfaces/objectPrediction-service.ts";
import { ObjectPredictionSearchParamsDTO } from "@/data/dto/objectPrediction-dto.ts";
import { ObjectPrediction } from "@/core/domain/ObjectPrediction.ts";

export function ObjectPredictionPage() {
  const [loading, setLoading] = useState(false);
  const [objectPredictions, setObjectPredictions] = useState<ObjectPrediction[]>([]);
  const [isSearchFormOpen, setIsSearchFormOpen] = useState(false);

  const objectPredictionService: IObjectPredictionService = useObjectPredictionService();
  const { success, error } = useToast();

  const {
    formData: searchFormData,
    setters: searchSetters,
    reset: resetSearch,
    getSearchParams,
  } = useObjectPredictionSearchForm();

  const fetchObjectPredictions = useCallback(
    async (params?: ObjectPredictionSearchParamsDTO) => {
      setLoading(true);
      try {
        const res = await objectPredictionService.getObjectPredictions(params);
        setObjectPredictions(res);
      } catch (err) {
        const apiError =
          err instanceof ApiError
            ? err
            : new ApiError("Failed to fetch Object Predictions", 500);
        error(apiError.message);
      } finally {
        setLoading(false);
      }
    },
    [objectPredictionService, error],
  );

  useEffect(() => {
    fetchObjectPredictions();
  }, []);

  const handleSearchSubmit = async () => {
    const params = getSearchParams();
    await fetchObjectPredictions(params);
    setIsSearchFormOpen(false);
    if (Object.keys(params).length > 0) {
      success("Search applied.");
    } else {
      success("Search cleared.");
    }
  };

  const handleRefresh = () => {
    resetSearch(); // Optionally reset search when refreshing all
    fetchObjectPredictions();
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

      <ObjectPredictionSearchForm
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

      <DataTable columns={columns} data={objectPredictions} />
      {/* No actions prop is passed to DataTable, so no action column will be rendered */}
    </div>
  );
}
