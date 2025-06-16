import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useToast } from "@/hooks/useToast";

interface RoboflowFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: () => Promise<void>;
  title: string;
  formData: {
    description: string;
    apiKey: string;
    modelUrl: string;
    localModel: File | null;
  };
  setters: {
    setDescription: (value: string) => void;
    setApiKey: (value: string) => void;
    setModelUrl: (value: string) => void;
    setLocalModel: (value: File | null) => void;
  };
}

export function RoboflowForm({
  isOpen,
  onClose,
  onSubmit,
  title,
  formData,
  setters,
}: RoboflowFormProps) {
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

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0] || null;
    setters.setLocalModel(file);
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>{title}</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="description">Description</Label>
              <Input
                id="description"
                placeholder="Enter description"
                value={formData.description}
                onChange={(e) => setters.setDescription(e.target.value)}
                required
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="apiKey">API Key</Label>
              <Input
                id="apiKey"
                type="password"
                placeholder="Enter API key"
                value={formData.apiKey}
                onChange={(e) => setters.setApiKey(e.target.value)}
                required
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="modelUrl">Model URL</Label>
              <Input
                id="modelUrl"
                placeholder="Enter model URL"
                value={formData.modelUrl}
                onChange={(e) => setters.setModelUrl(e.target.value)}
                required
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="localModelPath">Local Model File</Label>
              <Input
                id="localModelPath"
                type="file"
                accept=".pt,.onnx,.weights"
                onChange={handleFileChange}
                className="cursor-pointer"
              />
              {formData.localModel && (
                <p className="text-sm text-muted-foreground">
                  Selected: {formData.localModel.name}
                </p>
              )}
            </div>
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
