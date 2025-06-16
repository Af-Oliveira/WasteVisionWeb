import { Row } from "@tanstack/react-table";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { MoreHorizontal } from "lucide-react";
import { TableAction } from "./types";

interface TableActionsProps<T> {
  row: Row<T>;
  actions: TableAction<T>[];
}

export function TableActions<T>({ row, actions }: TableActionsProps<T>) {
  const visibleActions = actions.filter((action) => {
    return !action.hidden?.(row);
  });

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="ghost" className="h-8 w-8 p-0">
          <MoreHorizontal className="h-4 w-4" />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end">
        {visibleActions.map((action) => (
          <DropdownMenuItem
            key={action.id}
            onClick={() => action.onClick(row)}
            disabled={action.disabled}
          >
            {action.icon && <span className="mr-2 h-4 w-4">{action.icon}</span>}
            {action.label}
          </DropdownMenuItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
