import { FormValidationError } from "@/lib/utils";
import { User, UserSchema } from "@/core/domain/User";
import { UserSearchParamsDTO } from "@/data/dto/user-dto";
import { useState } from "react";

export function useUserForm(initialData?: Partial<User>) {
  const [email, setEmail] = useState(initialData?.email || "");
  const [username, setUserName] = useState(initialData?.username || "");
  const [roleId, setRoleId] = useState<string>(initialData?.role?.id || "");

  const reset = () => {
    setEmail("");
    setUserName("");
    setRoleId("");
  };

  const getFormData = (): User => ({
    email,
    username,
    role: {
      id: roleId,
      description: "fill",
      active: true,
    },
    active: true,
  });

  const isFormValid = (): boolean => {
    const formData = getFormData();
    const result = UserSchema.safeParse(formData);
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
      email,
      username,
      roleId,
    },
    setters: {
      setEmail,
      setUserName,
      setRoleId,
    },
    reset,
    isFormValid,
    getFormData,
  };
}

export function useUserSearchForm() {
  const [email, setEmail] = useState("");
  const [username, setUserName] = useState("");
  const [roleId, setRoleId] = useState("");

  const reset = () => {
    setEmail("");
    setUserName("");
    setRoleId("");
  };

  const getSearchParams = (): UserSearchParamsDTO => ({
    email: email || undefined,
    username: username || undefined,
    roleId: roleId || undefined,
  });

  return {
    formData: {
      email,
      username,
      roleId,
    },
    setters: {
      setEmail,
      setUserName,
      setRoleId,
    },
    reset,
    getSearchParams,
  };
}
