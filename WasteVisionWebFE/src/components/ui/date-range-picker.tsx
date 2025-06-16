// src/components/ui/DateRangePicker.tsx
import * as React from "react";
import { Label } from "@/components/ui/label";
import { SimpleDatePicker } from "./date-picker"; // Corrected import path
import { addDays } from "date-fns";

interface DateRangePickerProps {
  dateFrom: Date | undefined;
  dateTo: Date | undefined;
  onDateFromChange: (date: Date | undefined) => void;
  onDateToChange: (date: Date | undefined) => void;
  fromLabel?: string;
  toLabel?: string;
  fromPlaceholder?: string;
  toPlaceholder?: string;
  disabled?: boolean;
  maxRange?: number;
}

export function DateRangePicker({
  dateFrom,
  dateTo,
  onDateFromChange,
  onDateToChange,
  fromLabel = "From",
  toLabel = "To",
  fromPlaceholder = "Start date",
  toPlaceholder = "End date",
  disabled = false,
  maxRange,
}: DateRangePickerProps) {
  const handleDateFromChange = React.useCallback(
    (newDateFrom: Date | undefined) => {
      console.log("From date changing to:", newDateFrom);
      onDateFromChange(newDateFrom);
      
      // If the new "From" date is after the current "To" date, clear "To" date
      if (dateTo && newDateFrom && newDateFrom > dateTo) {
        onDateToChange(undefined);
      }
    },
    [onDateFromChange, onDateToChange, dateTo]
  );

  const handleDateToChange = React.useCallback(
    (newDateTo: Date | undefined) => {
      console.log("To date changing to:", newDateTo);
      onDateToChange(newDateTo);
    },
    [onDateToChange]
  );

  // Function to disable dates for the "To" picker
  const getDisabledDatesForTo = React.useCallback(
    (date: Date) => {
      if (!dateFrom) return false;
      
      // Disable dates before the "From" date
      if (date < dateFrom) return true;
      
      // If maxRange is specified, disable dates beyond the range
      if (maxRange && date > addDays(dateFrom, maxRange)) return true;
      
      return false;
    },
    [dateFrom, maxRange]
  );

  return (
    <div className="grid grid-cols-1 gap-x-4 gap-y-2 sm:grid-cols-2">
      <div className="space-y-1">
        <Label htmlFor="date-range-from" className="text-sm">
          {fromLabel}
        </Label>
        <SimpleDatePicker
          date={dateFrom}
          setDate={handleDateFromChange}
          placeholder={fromPlaceholder}
          disabled={disabled}
        />
      </div>
      <div className="space-y-1">
        <Label htmlFor="date-range-to" className="text-sm">
          {toLabel}
        </Label>
        <SimpleDatePicker
          date={dateTo}
          setDate={handleDateToChange}
          placeholder={toPlaceholder}
          disabled={disabled || !dateFrom}
          disabledDates={getDisabledDatesForTo}
        />
      </div>
    </div>
  );
}
