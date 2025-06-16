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
import { useRoleService } from "@/di/container.tsx";
import { useRoleForm, useRoleSearchForm } from "@/hooks/useRoleForm";
import { useToast } from "@/hooks/useToast";
import { FormValidationError } from "@/lib/utils";
import { Role } from "@/core/domain/Role.ts";
import { ApiError } from "@/data/http/httpClient.ts";
import { Copy, Edit, PowerOff, Trash2 } from "lucide-react";
import { useCallback, useEffect, useState } from "react";
import { TableHeader } from "../common/TableHeader.tsx";
import { TableAction } from "../common/types.ts";
import { columns } from "./columns.tsx";
import { RoleForm } from "./components/RoleForm.tsx";
import { RoleSearchForm } from "./components/RoleSearchForm.tsx";
import { DataTable } from "./data-table.tsx";
import { IRoleService } from "@/core/services/interfaces/role-service.ts";
import { RoleSearchParamsDTO } from "@/data/dto/role-dto.ts";

export function RolesPage() {
  // State management
  const [loading, setLoading] = useState(false);
  const [roles, setRoles] = useState<Role[]>([]);
  const [selectedRole, setSelectedRole] = useState<Role | null>(null);
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [isDeactivateDialogOpen, setIsDeactivateDialogOpen] = useState(false);
  const [isActivateDialogOpen, setIsActivateDialogOpen] = useState(false);
  const [isSearchFormOpen, setIsSearchFormOpen] = useState(false);

  // Services and hooks
  const roleService: IRoleService = useRoleService();
  const { success, error } = useToast();

  // Form hooks
  const { formData, setters, reset, isFormValid, getFormData } = useRoleForm();

  const {
    formData: searchFormData,
    setters: searchSetters,
    reset: resetSearch,
    getSearchParams,
  } = useRoleSearchForm();

  // Fetch roles with optional search parameters
  const fetchRoles = useCallback(
    async (params?: RoleSearchParamsDTO) => {
      setLoading(true);
      try {
        const res = await roleService.getRoles(params);
        setRoles(res);
        return true;
      } catch (err) {
        if (err instanceof ApiError) {
          throw err;
        }
        throw new ApiError("Failed to fetch", 500);
      } finally {
        setLoading(false);
      }
    },
    [roleService]
  );

  // Consolidated role action handler
  const handleRoleAction = async (
    actionType: "save" | "delete" | "activate" | "deactivate",
    roleId?: string
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
          const formData = getFormData();
          if (roleId) {
            await roleService.putRole(roleId, formData);
            successMessage = "Role updated successfully";
          } else {
            await roleService.postRole(formData);
            successMessage = "Role created successfully";
          }
          result = true;
          break;

        case "delete":
          if (roleId) {
            result = await roleService.deleteRole(roleId);
            successMessage = "Role deleted successfully";
          }
          break;

        case "activate":
          if (roleId) {
            result = await roleService.activateRole(roleId);
            successMessage = "Role activated successfully";
          }
          break;

        case "deactivate":
          if (roleId) {
            result = await roleService.deactivateRole(roleId);
            successMessage = "Role deactivated successfully";
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
        setSelectedRole(null);
        if (actionType === "save") reset();

        // Refresh roles and show success message
        await fetchRoles();
        success(successMessage);
      } else {
        error(`Failed to ${actionType} role.`);
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

  // Load roles on component mount
  useEffect(() => {
    fetchRoles();
  }, [fetchRoles]);

  // Event handlers - simplified
  const handleCreate = () => {
    setSelectedRole(null);
    reset();
    setIsFormOpen(true);
  };

  const handleEdit = (role: Role) => {
    setSelectedRole(role);
    setters.setDescription(role.description);
    setIsFormOpen(true);
  };

  const handleSubmit = () => handleRoleAction("save", selectedRole?.id);
  const handleDelete = () => handleRoleAction("delete", selectedRole?.id);
  const handleDeactivate = () =>
    handleRoleAction("deactivate", selectedRole?.id);
  const handleActivate = () => handleRoleAction("activate", selectedRole?.id);

  const handleCopyId = (id: string) => {
    navigator.clipboard.writeText(id);
    success("ID copied to clipboard!");
  };

  const handleSearch = async () => {
    try {
      const params = getSearchParams();
      await fetchRoles(params);
      setIsSearchFormOpen(false);
      success("Search completed successfully");
    } catch (err) {
      error(
        err instanceof Error ? err.message : "An unexpected error occurred"
      );
    }
  };

  const actions: TableAction<Role>[] = [
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
        setSelectedRole(row.original);
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
        setSelectedRole(row.original);
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
        setSelectedRole(row.original);
        setIsDeleteDialogOpen(true);
      },
      variant: "destructive",
      hidden: (row) => row.original.active,
    },
  ];

  return (
    <div className="space-y-4">
      <TableHeader
        title="Roles"
        onCreate={handleCreate}
        isLoading={loading}
        onRefresh={() => fetchRoles()}
        onSearch={() => setIsSearchFormOpen(true)}
      />

      <RoleSearchForm
        isOpen={isSearchFormOpen}
        onClose={() => setIsSearchFormOpen(false)}
        onSubmit={handleSearch}
        formData={searchFormData}
        setters={searchSetters}
        reset={resetSearch}
      />

      <DataTable columns={columns} data={roles} actions={actions} />

      <RoleForm
        isOpen={isFormOpen}
        onClose={() => {
          setIsFormOpen(false);
          setSelectedRole(null);
          reset();
        }}
        onSubmit={handleSubmit}
        title={selectedRole ? "Edit role" : "Create role"}
        formData={formData}
        setters={setters}
      />

      <AlertDialog open={isDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Are you sure?</AlertDialogTitle>
            <AlertDialogDescription>
              This action cannot be undone. This will permanently delete the
              user's account and remove their data from our servers.
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
            <AlertDialogTitle>Deactivate role?</AlertDialogTitle>
            <AlertDialogDescription>
              This will deactivate the user's account. They will no longer be
              able to access the system until reactivated.
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
            <AlertDialogTitle>Activate role?</AlertDialogTitle>
            <AlertDialogDescription>
              This will activate the role. It will be available for use in the
              system.
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
