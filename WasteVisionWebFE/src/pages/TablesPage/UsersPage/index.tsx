import { useState, useCallback, useEffect } from "react";
import { Edit, Trash2, PowerOff, Copy } from "lucide-react";
import { columns } from "./columns";
import { DataTable } from "./data-table";
import { TableHeader } from "../common/TableHeader";
import { TableAction } from "../common/types";
import { User } from "@/core/domain/User";
import { UserForm } from "./components/UserForm";
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
import { useToast } from "@/hooks/useToast";
import { useUserForm, useUserSearchForm } from "@/hooks/useUserForm";
import { UserSearchForm } from "./components/UserSearchForm";
import { useUserService, useRoleService } from "@/di/container";
import { ApiError } from "@/data/http/httpClient";
import { FormValidationError } from "@/lib/utils";
import { UserSearchParamsDTO } from "@/data/dto/user-dto";
import { IUserService } from "@/core/services/interfaces/user-service";
import { IRoleService } from "@/core/services/interfaces/role-service";
import { Role } from "@/core/domain/Role";
import { Console } from "console";

export function UsersPage() {
  // State management
  const [loading, setLoading] = useState(false);
  const [users, setUsers] = useState<User[]>([]);
  const [roles, setRoles] = useState<Role[]>([]);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [isDeactivateDialogOpen, setIsDeactivateDialogOpen] = useState(false);
  const [isActivateDialogOpen, setIsActivateDialogOpen] = useState(false);
  const [isSearchFormOpen, setIsSearchFormOpen] = useState(false);

  // Services and hooks
  const userService: IUserService = useUserService();
  const roleService: IRoleService = useRoleService();
  const { success, error } = useToast();

  // Form hooks
  const { formData, setters, reset, isFormValid, getFormData } = useUserForm();

  const {
    formData: searchFormData,
    setters: searchSetters,
    reset: resetSearch,
    getSearchParams,
  } = useUserSearchForm();

  // Fetch users with optional search parameters
  const fetchUsers = useCallback(
    async (params?: UserSearchParamsDTO) => {
   
      setLoading(true);
      try {
        const res = await userService.getUsers(params);
        setUsers(res);
        return true;
      } catch (err) {
        if (err instanceof ApiError) {
          throw err;
        }
        throw new ApiError("Failed to fetch users", 500);
      } finally {
        setLoading(false);
      }
    },
    [userService]
  );

  // Fetch roles for dropdowns
  const fetchRoles = useCallback(async () => {
    try {
      const res = await roleService.getRoles();
      setRoles(res);
    } catch (err) {
      console.error("Failed to fetch roles", err);
    }
  }, [roleService]);

  // Load data on component mount
  useEffect(() => {
    fetchRoles();
    fetchUsers();
  }, [fetchUsers, fetchRoles]);

  // Consolidated user action handler
  const handleUserAction = async (
    actionType: "save" | "delete" | "activate" | "deactivate",
    userId?: string
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
          if (userId) {
            await userService.putUser(userId, formData);
            successMessage = "User updated successfully";
          } else {
            await userService.postUser(formData);
            successMessage = "User created successfully";
          }
          result = true;
          break;

        case "delete":
          if (userId) {
            result = await userService.deleteUser(userId);
            successMessage = "User deleted successfully";
          }
          break;

        case "activate":
          if (userId) {
            result = await userService.activateUser(userId);
            successMessage = "User activated successfully";
          }
          break;

        case "deactivate":
          if (userId) {
            result = await userService.deactivateUser(userId);
            successMessage = "User deactivated successfully";
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
        setSelectedUser(null);
        if (actionType === "save") reset();

        // Refresh users and show success message
        await fetchUsers();
        success(successMessage);
      } else {
        error(`Failed to ${actionType} user.`);
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

  // Event handlers - simplified
  const handleCreate = () => {
    setSelectedUser(null);
    reset();
    setIsFormOpen(true);
  };

  const handleEdit = (user: User) => {
    setSelectedUser(user);
    // Set form data using setters
    setters.setUserName(user.username);
    setters.setEmail(user.email);
    setters.setRoleId(user.role?.id || "");
    setIsFormOpen(true);
  };

  const handleSubmit = () => handleUserAction("save", selectedUser?.id);
  const handleDelete = () => handleUserAction("delete", selectedUser?.id);
  const handleDeactivate = () =>
    handleUserAction("deactivate", selectedUser?.id);
  const handleActivate = () => handleUserAction("activate", selectedUser?.id);

  const handleSearch = async () => {
    try {
      const params = getSearchParams();
      await fetchUsers(params);
      setIsSearchFormOpen(false);
      success("Search completed successfully");
    } catch (err) {
      error(
        err instanceof Error ? err.message : "An unexpected error occurred"
      );
    }
  };

  const handleCopyId = (id: string) => {
    navigator.clipboard.writeText(id);
    success("ID copied to clipboard!");
  };

  const actions: TableAction<User>[] = [
    {
      id: "copy",
      label: "Copy ID",
      icon: <Copy className="h-4 w-4" />,
      onClick: (row) => handleCopyId(row.original.id || ""),
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
        setSelectedUser(row.original);
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
        setSelectedUser(row.original);
        setIsActivateDialogOpen(true);
      },
      hidden: (row) => row.original?.active || false,
      variant: "default",
    },
    {
      id: "delete",
      label: "Delete",
      icon: <Trash2 className="h-4 w-4" />,
      onClick: (row) => {
        setSelectedUser(row.original);
        setIsDeleteDialogOpen(true);
      },
      hidden: (row) => row.original?.active || false,
      variant: "destructive",
    },
  ];

  return (
    <div className="space-y-4">
      <TableHeader
        title="Users"
        onCreate={handleCreate}
        isLoading={loading}
        onRefresh={() => fetchUsers()}
        onSearch={() => setIsSearchFormOpen(true)}
      />

      <UserSearchForm
        isOpen={isSearchFormOpen}
        onClose={() => setIsSearchFormOpen(false)}
        onSubmit={handleSearch}
        formData={searchFormData}
        setters={searchSetters}
        reset={resetSearch}
        foreignData={{ roles }} 
        title={"Users Search"}      />

      <DataTable columns={columns} data={users} actions={actions} />

      <UserForm
        isOpen={isFormOpen}
        onClose={() => {
          setIsFormOpen(false);
          setSelectedUser(null);
          reset();
        }}
        onSubmit={handleSubmit}
        title={selectedUser ? "Edit User" : "Create User"}
        formData={formData}
        setters={setters}
        foreignData={{ roles }}
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
            <AlertDialogTitle>Deactivate user?</AlertDialogTitle>
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
            <AlertDialogTitle>Activate user?</AlertDialogTitle>
            <AlertDialogDescription>
              This will activate the user. It will be available for use in the
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
