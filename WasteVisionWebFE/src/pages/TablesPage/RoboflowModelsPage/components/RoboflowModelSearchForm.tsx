import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

interface RoboflowSearchFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: () => void;
  formData: {
    description: string;
    modelUrl: string;
    active: string
  };
  setters: {
    setDescription: (value: string) => void;
    setModelUrl: (value: string) => void;
    setActive: (value: string) => void;
  };
  reset: () => void;
}

export function RoboflowSearchForm({
  isOpen,
  onClose,
  onSubmit,
  formData,
  setters,
  reset,
}: RoboflowSearchFormProps) {
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit();
  };

  const handleReset = () => {
    reset();
  };


  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Search Roboflow Models</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Input
              id="search-description"
              placeholder="Search by description"
              value={formData.description}
              onChange={(e) => setters.setDescription(e.target.value)}
            />
            <Input
              id="search-modelUrl"
              placeholder="Search by model URL"
              value={formData.modelUrl}
              onChange={(e) => setters.setModelUrl(e.target.value)}
            />

            <Select
              value={formData.active}
              onValueChange={(value) => setters.setActive(value)}
            >
              <SelectTrigger>
                <SelectValue placeholder="Select State" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="1">Active</SelectItem>
                <SelectItem value="0">Deactivated</SelectItem>
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
