import { type Row } from "@tanstack/react-table";

export interface TableAction<T> {
  id: string;
  label: string;
  onClick: (row: Row<T>) => void;
  icon?: React.ReactNode;
  disabled?: boolean;
  variant?: "default" | "destructive" | "outline" | "ghost";
  hidden?: (row: Row<T>) => boolean;
}

export interface BaseTableProps<T> {
  data: T[];
  isLoading: boolean;
  error?: string;
  onRefresh: () => void;
}

export interface TableOperations<T> {
  create: () => void;
  update: (data: T) => Promise<void>;
  delete: (id: string) => Promise<void>;
  deactivate: (id: string) => Promise<void>;
}
