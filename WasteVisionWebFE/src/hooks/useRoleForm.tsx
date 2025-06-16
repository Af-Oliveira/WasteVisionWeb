import { FormValidationError } from "@/lib/utils";
import { Role, RoleSchema } from "@/core/domain/Role";
import { RoleSearchParamsDTO } from "@/data/dto/role-dto";
import { useState } from "react";

export function useRoleForm(initialData?: Partial<Role>) {
  const [description, setDescription] = useState(
    initialData?.description || ""
  );

  const reset = () => {
    setDescription("");
  };

  const getFormData = (): Role => ({
    description,
    active: true,
  });

  const isFormValid = (): boolean => {
    const formData = getFormData();
    const result = RoleSchema.safeParse(formData);
    if (!result.success) {
      const firstError = result.error.errors[0];
      throw new FormValidationError(
        firstError.path[0].toString(),
        firstError.message
      );
    }
    return true;
  };

  return {
    formData: {
      description,
    },
    setters: {
      setDescription,
    },
    reset,
    isFormValid,
    getFormData,
  };
}

export function useRoleSearchForm() {
  const [description, setDescription] = useState("");

  const reset = () => {
    setDescription("");
  };

  const getSearchParams = (): RoleSearchParamsDTO => ({
    description: description || undefined,
  });

  return {
    formData: {
      description,
    },
    setters: {
      setDescription,
    },
    reset,
    getSearchParams,
  };
}
