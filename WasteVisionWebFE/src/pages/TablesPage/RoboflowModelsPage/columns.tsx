import { RoboflowModel } from "@/core/domain/RoboflowModel";
import { ColumnDef } from "@tanstack/react-table";

export const columns: ColumnDef<RoboflowModel>[] = [
  {
    accessorKey: "description",
    header: "Description",
  },
  {
    accessorKey: "apiKey",
    header: "API Key",
    cell: ({ row }) => {
      const apiKey = row.getValue("apiKey") as string;
      // Mask the API key for security
      return apiKey ? `${apiKey.substring(0, 8)}...` : "N/A";
    },
  },
  {
    accessorKey: "modelUrl",
    header: "Model URL",
  },
    {
    accessorKey: "precision",
    header: "Precision",
  },
  {
    accessorKey: "localModelPath",
    header: "Local Model",
    cell: ({ row }) => {
      const path = row.getValue("localModelPath") as string;
      return path ? "📁 File uploaded" : "❌ No file";
    },
  },
  {
    accessorKey: "active",
    header: "Active",
    cell: ({ row }) => (row.getValue("active") ? "✅" : "❌"),
  },
];
