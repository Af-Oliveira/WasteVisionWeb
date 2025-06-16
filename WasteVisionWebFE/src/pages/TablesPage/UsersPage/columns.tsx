import { User } from "@/core/domain/User";
import { ColumnDef } from "@tanstack/react-table";

export const columns: ColumnDef<User>[] = [
  {
    accessorKey: "email",
    header: "Email",
  },
  {
    accessorKey: "role.description",
    header: "Role",
  },
   {
    accessorKey: "username",
    header: "UserName",
  },
  {
    accessorKey: "active",
    header: "Active",
    cell: ({ row }) => (row.getValue("active") ? "✅" : "❌"),
  },
];
