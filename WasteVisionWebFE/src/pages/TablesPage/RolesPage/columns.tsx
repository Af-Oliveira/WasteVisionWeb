import { Role } from "@/core/domain/Role";
import { ColumnDef } from "@tanstack/react-table";

export const columns: ColumnDef<Role>[] = [
  {
    accessorKey: "description",
    header: "Description",
  },
  {
    accessorKey: "active",
    header: "Active",

    cell: ({ row }) => (row.getValue("active") ? "✅" : "❌"),
  },
];
