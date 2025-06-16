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
import { Role } from "@/core/domain/Role";


interface UserSearchFormProps {
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
  reset: () => void;
  foreignData: {
    roles: Role[];
  };
}

const ALL_ROLES_VALUE = "all";

export function UserSearchForm({
  isOpen,
  onClose,
  onSubmit,
  formData,
  setters,
  reset,
  foreignData,
}: UserSearchFormProps) {
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit();
  };

  const handleReset = () => {
    reset();
  };

  const handleRoleChange = (value: string) => {
    setters.setRoleId(value === ALL_ROLES_VALUE ? "" : value);
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Search Users</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
           <Input
              placeholder="Email"
              value={formData.email}
              onChange={(e) => setters.setEmail(e.target.value)}
            />
            <Input
              placeholder="User name"
              value={formData.username}
              onChange={(e) => setters.setUserName(e.target.value)}
            />
            <Select
              value={formData.roleId || ALL_ROLES_VALUE}
              onValueChange={handleRoleChange}
            >
              <SelectTrigger>
                <SelectValue placeholder="Select a role" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value={ALL_ROLES_VALUE}>All Roles</SelectItem>
                {foreignData.roles.map((role) => (
              <SelectItem key={role.id} value={role.id ?? ''}>
                    {role.description}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
          <div className="flex justify-end gap-2">
            <Button type="button" variant="outline" onClick={handleReset}>
              Reset
            </Button>
            <Button type="button" variant="outline" onClick={onClose}>
              Cancel
            </Button>
            <Button type="submit">Search</Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}
