// src/components/ui/SimplePrecisionSlider.tsx
import { Input } from "@/components/ui/input";
import { cn } from "@/lib/utils";
import * as React from "react";

interface SimplePrecisionSliderProps {
  precision: number | undefined;
  setPrecision: (precision: number | undefined) => void;
  placeholder?: string;
  disabled?: boolean;
  step?: number;
  min?: number;
  max?: number;
}

export function SimplePrecisionSlider({
  precision,
  setPrecision,
  placeholder = "0.00",
  disabled = false,
  step = 0.01,
  min = 0,
  max = 1,
}: SimplePrecisionSliderProps) {
  const [inputValue, setInputValue] = React.useState<string>(
    precision !== undefined ? precision.toFixed(2) : ""
  );

  // Update input value when precision prop changes
  React.useEffect(() => {
    setInputValue(precision !== undefined ? precision.toFixed(2) : "");
  }, [precision]);

  const handleSliderChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = parseFloat(e.target.value);
    console.log("Slider precision selected:", value);
    setPrecision(value);
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setInputValue(value);

    // Validate and update precision
    const numericValue = parseFloat(value);
    if (!isNaN(numericValue) && numericValue >= min && numericValue <= max) {
      setPrecision(parseFloat(numericValue.toFixed(2)));
    } else if (value === "") {
      setPrecision(undefined);
    }
  };

  const handleInputBlur = () => {
    // Ensure the input shows the correct formatted value
    if (precision !== undefined) {
      setInputValue(precision.toFixed(2));
    }
  };

  return (
    <div className="space-y-3">
      {/* Slider */}
      <div className="relative">
        <input
          type="range"
          min={min}
          max={max}
          step={step}
          value={precision ?? min}
          onChange={handleSliderChange}
          disabled={disabled}
          className={cn(
            "w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer",
            "slider-thumb:appearance-none slider-thumb:h-4 slider-thumb:w-4",
            "slider-thumb:rounded-full slider-thumb:bg-blue-600",
            "slider-thumb:cursor-pointer slider-thumb:border-0",
            "focus:outline-none focus:ring-2 focus:ring-blue-500",
            disabled && "opacity-50 cursor-not-allowed"
          )}
          style={{
            background: `linear-gradient(to right, #3b82f6 0%, #3b82f6 ${
              ((precision ?? min) - min) / (max - min) * 100
            }%, #e5e7eb ${
              ((precision ?? min) - min) / (max - min) * 100
            }%, #e5e7eb 100%)`,
          }}
        />
      </div>

      {/* Input */}
     

      {/* Value display */}
      {precision !== undefined && (
        <div className="text-center">
          <span className="text-sm font-medium">
            Selected: {precision.toFixed(2)}
          </span>
        </div>
      )}
    </div>
  );
}
