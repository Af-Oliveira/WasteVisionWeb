// src/features/predictions/components/PredictionSearchForm.tsx
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
  DialogDescription,
} from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { PrecisionRangePicker } from "@/components/ui/precision-range-picker";
import React from "react";

interface ObjectPredictionSearchForm {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: () => void;
  formData: {
    precisionFrom: number | undefined; // Keep as number | undefined
    precisionTo: number | undefined;   // Keep as number | undefined
  };
  setters: {
    setPrecisionFrom: (value: number | undefined) => void;
    setPrecisionTo: (value: number | undefined) => void;
  };
  reset: () => void;
}

export function ObjectPredictionSearchForm({
  isOpen,
  onClose,
  onSubmit,
  formData,
  setters,
  reset,
}: ObjectPredictionSearchForm) {
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit();
  };

  const handleReset = () => {
    reset();
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Search Predictions</DialogTitle>
          <DialogDescription>
            Filter object prediction by precision.
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-6 py-4">
          {/* Model ID Input */}
      
          {/* Precision Range Picker Section */}
          <div className="space-y-2">
            <Label>Precision Range</Label>
            <PrecisionRangePicker
              precisionFrom={formData.precisionFrom}
              precisionTo={formData.precisionTo}
              onPrecisionFromChange={setters.setPrecisionFrom}
              onPrecisionToChange={setters.setPrecisionTo}
              fromLabel="Min Precision"
              toLabel="Max Precision"
              fromPlaceholder="0.00"
              toPlaceholder="1.00"
              step={0.01}
              min={0}
              max={1}
            />
          </div>

          <DialogFooter className="gap-2 sm:justify-end pt-4">
            <Button type="button" variant="outline" onClick={handleReset}>
              Reset
            </Button>
            <Button type="button" variant="ghost" onClick={onClose}>
              Cancel
            </Button>
            <Button type="submit">Search</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
