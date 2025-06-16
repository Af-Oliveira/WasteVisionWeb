// src/components/ui/SimpleDatePicker.tsx
import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { cn } from "@/lib/utils";
import { CalendarIcon } from "lucide-react";
import { format } from "date-fns";
import * as React from "react";

interface SimpleDatePickerProps {
  date: Date | undefined;
  setDate: (date: Date | undefined) => void;
  placeholder?: string;
  disabled?: boolean;
  disabledDates?: (date: Date) => boolean;
}

export function SimpleDatePicker({
  date,
  setDate,
  placeholder = "Pick a date",
  disabled = false,
  disabledDates,
}: SimpleDatePickerProps) {
  const [open, setOpen] = React.useState(false);

  const handleDateSelect = (selectedDate: Date | undefined) => {
    console.log("Date selected:", selectedDate); // Debug log
    setDate(selectedDate);
    setOpen(false);
  };

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <Button
          variant="outline"
          className={cn(
            "w-full justify-start text-left font-normal",
            !date && "text-muted-foreground"
          )}
          disabled={disabled}
        >
          <CalendarIcon className="mr-2 h-4 w-4" />
          {date ? format(date, "PPP") : <span>{placeholder}</span>}
        </Button>
      </PopoverTrigger>
      <PopoverContent 
        className="w-auto p-0" 
        align="start"
        style={{ zIndex: 9999, pointerEvents: 'auto' }}
      >
        <Calendar
          mode="single"
          selected={date}
          onSelect={handleDateSelect}
          disabled={disabledDates}
          initialFocus
        />
      </PopoverContent>
    </Popover>
  );
}
