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
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import React from "react";

// Import the new DateRangePicker
import { DateRangePicker } from "@/components/ui/date-range-picker"; // Adjust path as needed

interface PredictionSearchFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: () => void;
  formData: {
    modelName: string;
    userName: string;
    dateFrom: Date | undefined; // Keep as Date | undefined
    dateTo: Date | undefined;   // Keep as Date | undefined
  };
  setters: {
    setModelName: (value: string) => void;
    setUserName: (value: string) => void;
    setDateFrom: (date: Date | undefined) => void;
    setDateTo: (date: Date | undefined) => void;
  };
  reset: () => void;
}

export function PredictionSearchForm({
  isOpen,
  onClose,
  onSubmit,
  formData,
  setters,
  reset,
}: PredictionSearchFormProps) {
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
            Filter predictions by Model, User , and date range.
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-6 py-4">
          {/* Model ID Input */}
          <div className="space-y-2">
            <Label htmlFor="search-modelName">Model Name</Label>
            <Input
              id="search-modelName"
              placeholder="Enter Model"
              value={formData.modelName}
              onChange={(e) => setters.setModelName(e.target.value)}
            />
          </div>

          {/* User ID Input */}
          <div className="space-y-2">
            <Label htmlFor="search-userName">User Name</Label>
            <Input
              id="search-userName"
              placeholder="Enter User "
              value={formData.userName}
              onChange={(e) => setters.setUserName(e.target.value)}
            />
          </div>

          {/* Date Range Picker Section */}
          <div className="space-y-2">
            <Label>Date Range</Label>
            <DateRangePicker
              dateFrom={formData.dateFrom}
              dateTo={formData.dateTo}
              onDateFromChange={setters.setDateFrom}
              onDateToChange={setters.setDateTo}
            />
            {/* Optional: Inline validation message */}
            {formData.dateFrom &&
              formData.dateTo &&
              formData.dateFrom > formData.dateTo && (
                <p className="text-xs text-destructive pt-1">
                  "Date From" cannot be after "Date To".
                </p>
              )}
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
