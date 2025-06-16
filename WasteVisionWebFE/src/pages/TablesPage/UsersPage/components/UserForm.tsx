import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useToast } from "@/hooks/useToast";
import { Role } from "@/core/domain/Role";
import { useState } from "react";

interface UserFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: () => Promise<void>;
  title: string;
  formData: {
    email: string;
    username: string;
    roleId: string;
  };
  setters: {
    setEmail: (value: string) => void;
    setUserName: (value: string) => void;
    setRoleId: (value: string) => void;
  };
  foreignData: {
    roles: Role[];
  };
}

export function UserForm({
  isOpen,
  onClose,
  onSubmit,
  title,
  formData,
  setters,
  foreignData,
}: UserFormProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { error } = useToast();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    try {
      await onSubmit();
      onClose();
    } catch (err) {
      error(
        err instanceof Error ? err.message : "An unexpected error occurred"
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{title}</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Input
              placeholder="Email"
              value={formData.email}
              onChange={(e) => setters.setEmail(e.target.value)}
              required
            />
            <Input
              placeholder="User name"
              value={formData.username}
              onChange={(e) => setters.setUserName(e.target.value)}
              required
            />
            <Select
              value={formData.roleId}
              onValueChange={setters.setRoleId}
              required
            >
              <SelectTrigger>
                <SelectValue placeholder="Select a role" />
              </SelectTrigger>
              <SelectContent>
                {foreignData.roles.map((role) => (
                  <SelectItem key={role.id} value={role.id ?? ''}>
                    {role.description}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
          <div className="flex justify-end gap-2">
            <Button type="button" variant="outline" onClick={onClose}>
              Cancel
            </Button>
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting ? "Saving..." : "Save"}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}
