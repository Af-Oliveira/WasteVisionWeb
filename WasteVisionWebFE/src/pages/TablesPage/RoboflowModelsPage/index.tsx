import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { useRoboflowModelService } from "@/di/container.tsx";
import { useRoboflowForm, useRoboFlowModelSearchForm } from "@/hooks/useRoboflowModelForm";
import { useToast } from "@/hooks/useToast";
import { FormValidationError } from "@/lib/utils";
import { RoboflowModel } from "@/core/domain/RoboflowModel.ts";
import { ApiError } from "@/data/http/httpClient.ts";
import { Copy, Edit, PowerOff, Trash2 } from "lucide-react";
import { useCallback, useEffect, useState } from "react";
import { TableHeader } from "../common/TableHeader.tsx";
import { TableAction } from "../common/types.ts";
import { columns } from "./columns.tsx";
import { RoboflowForm } from "./components/RoboflowForm.tsx";
import { RoboflowSearchForm } from "./components/RoboflowModelSearchForm.tsx";
import { DataTable } from "./data-table.tsx";
import { IRoboflowModelService } from "@/core/services/interfaces/roboflowModel-service.ts";
import { RoboflowModelSearchParamsDTO } from "@/data/dto/roboflowModel-dto.ts";

export function RoboflowModelsPage() {
  // State management
  const [loading, setLoading] = useState(false);
  const [roboflowModels, setRoboflowModels] = useState<RoboflowModel[]>([]);
  const [selectedModel, setSelectedModel] = useState<RoboflowModel | null>(null);
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [isDeactivateDialogOpen, setIsDeactivateDialogOpen] = useState(false);
  const [isActivateDialogOpen, setIsActivateDialogOpen] = useState(false);
  const [isSearchFormOpen, setIsSearchFormOpen] = useState(false);

  // Services and hooks
  const roboflowModelService: IRoboflowModelService = useRoboflowModelService();
  const { success, error } = useToast();

  // Form hooks
  const { 
    formData, 
    setters, 
    reset, 
    isFormValid, 
    getCreateFormData, 
    getUpdateFormData 
  } = useRoboflowForm();

  const {
    formData: searchFormData,
    setters: searchSetters,
    reset: resetSearch,
    getSearchParams,
  } = useRoboFlowModelSearchForm();

  // Fetch models with optional search parameters
  const fetchModels = useCallback(
    async (params?: RoboflowModelSearchParamsDTO) => {
      setLoading(true);
      try {
        const res = await roboflowModelService.getRoboflowModels(params);
        setRoboflowModels(res);
        return true;
      } catch (err) {
        if (err instanceof ApiError) {
          throw err;
        }
        throw new ApiError("Failed to fetch models", 500);
      } finally {
        setLoading(false);
      }
    },
    [roboflowModelService]
  );

  // Consolidated model action handler
  const handleModelAction = async (
    actionType: "save" | "delete" | "activate" | "deactivate",
    modelId?: string
  ): Promise<void> => {
    try {
      setLoading(true);

      let result = false;
      let successMessage = "";

      // For save action, validate first
      if (actionType === "save") {
        try {
          isFormValid();
        } catch (err) {
          if (err instanceof FormValidationError) {
            throw new ApiError(err.message, 422);
          }
          throw new ApiError("Validation failed", 422);
        }
      }

      // Perform the action
      switch (actionType) {
        case "save":
          if (modelId) {
            // Update existing model
            const updateData = getUpdateFormData();
            await roboflowModelService.putRoboflowModel(modelId, updateData);
            successMessage = "Model updated successfully";
          } else {
            // Create new model
            const createData = getCreateFormData();
            await roboflowModelService.postRoboflowModel(createData);
            successMessage = "Model created successfully";
          }
          result = true;
          break;

        case "delete":
          if (modelId) {
            result = await roboflowModelService.deleteRoboflowModel(modelId);
            successMessage = "Model deleted successfully";
          }
          break;

        case "activate":
          if (modelId) {
            result = await roboflowModelService.activateRoboflowModel(modelId);
            successMessage = "Model activated successfully";
          }
          break;

        case "deactivate":
          if (modelId) {
            result = await roboflowModelService.deactivateRoboflowModel(modelId);
            successMessage = "Model deactivated successfully";
          }
          break;
      }

      // Handle result
      if (result) {
        // Close corresponding dialog
        setIsFormOpen(false);
        setIsDeleteDialogOpen(false);
        setIsActivateDialogOpen(false);
        setIsDeactivateDialogOpen(false);

        // Clear selection and reset form if needed
        setSelectedModel(null);
        if (actionType === "save") reset();

        // Refresh models and show success message
        await fetchModels();
        success(successMessage);
      } else {
        error(`Failed to ${actionType} model.`);
      }
    } catch (e) {
      if (e instanceof ApiError && e.message) {
        error(e.message);
      } else {
        error(`An error occurred while performing ${actionType} action.`);
      }
    } finally {
      setLoading(false);
    }
  };

  // Load models on component mount
  useEffect(() => {
    fetchModels();
  }, [fetchModels]);

  // Event handlers
  const handleCreate = () => {
    setSelectedModel(null);
    reset();
    setIsFormOpen(true);
  };

  const handleEdit = (model: RoboflowModel) => {
    setSelectedModel(model);
    setters.setDescription(model.description || "");
    setters.setApiKey(model.apiKey || "");
    setters.setModelUrl(model.modelUrl || "");
    // Note: File cannot be pre-populated for security reasons
    setters.setLocalModel(null);
    setIsFormOpen(true);
  };

  const handleSubmit = () => handleModelAction("save", selectedModel?.id);
  const handleDelete = () => handleModelAction("delete", selectedModel?.id);
  const handleDeactivate = () => handleModelAction("deactivate", selectedModel?.id);
  const handleActivate = () => handleModelAction("activate", selectedModel?.id);

  const handleCopyId = (id: string) => {
    navigator.clipboard.writeText(id);
    success("ID copied to clipboard!");
  };

  const handleSearch = async () => {
    try {
      const params = getSearchParams();
      await fetchModels(params);
      setIsSearchFormOpen(false);
      success("Search completed successfully");
    } catch (err) {
      error(
        err instanceof Error ? err.message : "An unexpected error occurred"
      );
    }
  };

  const actions: TableAction<RoboflowModel>[] = [
    {
      id: "copy",
      label: "Copy ID",
      icon: <Copy className="h-4 w-4" />,
      onClick: (row) => handleCopyId(row.original?.id || ""),
      variant: "outline",
    },
    {
      id: "edit",
      label: "Edit",
      icon: <Edit className="h-4 w-4" />,
      onClick: (row) => handleEdit(row.original),
    },
    {
      id: "deactivate",
      label: "Deactivate",
      icon: <PowerOff className="h-4 w-4" />,
      onClick: (row) => {
        setSelectedModel(row.original);
        setIsDeactivateDialogOpen(true);
      },
      hidden: (row) => !row.original.active,
      variant: "outline",
    },
    {
      id: "activate",
      label: "Activate",
      icon: <PowerOff className="h-4 w-4" />,
      onClick: (row) => {
        setSelectedModel(row.original);
        setIsActivateDialogOpen(true);
      },
      hidden: (row) => row.original.active,
      variant: "default",
    },
    {
      id: "delete",
      label: "Delete",
      icon: <Trash2 className="h-4 w-4" />,
      onClick: (row) => {
        setSelectedModel(row.original);
        setIsDeleteDialogOpen(true);
      },
      variant: "destructive",
      hidden: (row) => row.original.active,
    },
  ];

  return (
    <div className="space-y-4">
      <TableHeader
        title="Roboflow Models"
        onCreate={handleCreate}
        isLoading={loading}
        onRefresh={() => fetchModels()}
        onSearch={() => setIsSearchFormOpen(true)}
      />

      <RoboflowSearchForm
        isOpen={isSearchFormOpen}
        onClose={() => setIsSearchFormOpen(false)}
        onSubmit={handleSearch}
        formData={searchFormData}
        setters={searchSetters}
        reset={resetSearch}
      />

      <DataTable columns={columns} data={roboflowModels} actions={actions} />

      <RoboflowForm
        isOpen={isFormOpen}
        onClose={() => {
          setIsFormOpen(false);
          setSelectedModel(null);
          reset();
        }}
        onSubmit={handleSubmit}
        title={selectedModel ? "Edit Roboflow Model" : "Create Roboflow Model"}
        formData={formData}
        setters={setters}
      />

      {/* Alert Dialogs remain the same */}
      <AlertDialog open={isDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Are you sure?</AlertDialogTitle>
            <AlertDialogDescription>
              This action cannot be undone. This will permanently delete the
              Roboflow model and remove its data from our servers.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel onClick={() => setIsDeleteDialogOpen(false)}>
              Cancel
            </AlertDialogCancel>
            <AlertDialogAction onClick={handleDelete}>Delete</AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      <AlertDialog open={isDeactivateDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Deactivate model?</AlertDialogTitle>
            <AlertDialogDescription>
              This will deactivate the Roboflow model. It will no longer be
              available for detection until reactivated.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel onClick={() => setIsDeactivateDialogOpen(false)}>
              Cancel
            </AlertDialogCancel>
            <AlertDialogAction onClick={handleDeactivate}>
              Deactivate
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      <AlertDialog open={isActivateDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Activate model?</AlertDialogTitle>
            <AlertDialogDescription>
              This will activate the Roboflow model. It will be available for
              use in object detection.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel onClick={() => setIsActivateDialogOpen(false)}>
              Cancel
            </AlertDialogCancel>
            <AlertDialogAction onClick={handleActivate}>
              Activate
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
