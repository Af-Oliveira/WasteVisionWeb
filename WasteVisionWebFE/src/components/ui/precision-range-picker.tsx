// src/components/ui/PrecisionRangePicker.tsx
import * as React from "react";
import { Label } from "@/components/ui/label";
import { SimplePrecisionSlider } from "./precision-picker";

interface PrecisionRangePickerProps {
  precisionFrom: number | undefined;
  precisionTo: number | undefined;
  onPrecisionFromChange: (precision: number | undefined) => void;
  onPrecisionToChange: (precision: number | undefined) => void;
  fromLabel?: string;
  toLabel?: string;
  fromPlaceholder?: string;
  toPlaceholder?: string;
  disabled?: boolean;
  step?: number;
  min?: number;
  max?: number;
}

export function PrecisionRangePicker({
  precisionFrom,
  precisionTo,
  onPrecisionFromChange,
  onPrecisionToChange,
  fromLabel = "Min Precision",
  toLabel = "Max Precision",
  fromPlaceholder = "0.00",
  toPlaceholder = "1.00",
  disabled = false,
  step = 0.01,
  min = 0,
  max = 1,
}: PrecisionRangePickerProps) {
  const handlePrecisionFromChange = React.useCallback(
    (newPrecisionFrom: number | undefined) => {
      console.log("From precision changing to:", newPrecisionFrom);
      onPrecisionFromChange(newPrecisionFrom);

      // If the new "From" precision is greater than current "To" precision, update "To"
      if (
        precisionTo &&
        newPrecisionFrom &&
        newPrecisionFrom > precisionTo
      ) {
        onPrecisionToChange(newPrecisionFrom);
      }
    },
    [onPrecisionFromChange, onPrecisionToChange, precisionTo]
  );

  const handlePrecisionToChange = React.useCallback(
    (newPrecisionTo: number | undefined) => {
      console.log("To precision changing to:", newPrecisionTo);
      onPrecisionToChange(newPrecisionTo);

      // If the new "To" precision is less than current "From" precision, update "From"
      if (
        precisionFrom &&
        newPrecisionTo &&
        newPrecisionTo < precisionFrom
      ) {
        onPrecisionFromChange(newPrecisionTo);
      }
    },
    [onPrecisionToChange, onPrecisionFromChange, precisionFrom]
  );

  return (
    <div className="grid grid-cols-1 gap-x-4 gap-y-2 sm:grid-cols-2">
      <div className="space-y-1">
        <Label htmlFor="precision-range-from" className="text-sm">
          {fromLabel}
        </Label>
        <SimplePrecisionSlider
          precision={precisionFrom}
          setPrecision={handlePrecisionFromChange}
          placeholder={fromPlaceholder}
          disabled={disabled}
          step={step}
          min={min}
          max={precisionTo ?? max}
        />
      </div>
      <div className="space-y-1">
        <Label htmlFor="precision-range-to" className="text-sm">
          {toLabel}
        </Label>
        <SimplePrecisionSlider
          precision={precisionTo}
          setPrecision={handlePrecisionToChange}
          placeholder={toPlaceholder}
          disabled={disabled}
          step={step}
          min={precisionFrom ?? min}
          max={max}
        />
      </div>
    </div>
  );
}
