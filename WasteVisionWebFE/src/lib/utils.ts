import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";
import { createLogger } from "./LoggerFactory";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export class FormValidationError extends Error {
  constructor(public field: string, message: string) {
    const fieldName = field
      .replace(/([A-Z])/g, " $1")
      .replace(/^./, (str) => str.toUpperCase());
    super(`${fieldName}: ${message}`);
    this.name = "FormValidationError";
  }
}

export const logger = createLogger();
