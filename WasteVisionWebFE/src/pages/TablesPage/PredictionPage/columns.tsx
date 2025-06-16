// src/features/predictions/components/columns.tsx
import { Prediction } from "@/core/domain/Prediction";
import { ColumnDef, Row } from "@tanstack/react-table";

export const columns: ColumnDef<Prediction>[] = [

  {
    accessorKey: "user.username",
    header: "User",
  },
  {
    accessorKey: "roboflowModel.description",
    header: "Model",
  },
  {
    accessorKey: "date",
    header: "Date",
  },
  {
    accessorKey: "originalImageUrl",
    header: "Original Image",
    cell: ({ row }) => {
      const imageUrl = row.getValue("originalImageUrl") as string;
      if (!imageUrl) return "N/A";
      return (
        <a href={imageUrl} target="_blank" rel="noopener noreferrer">
          <img
            src={imageUrl}
            alt="Original Prediction"
            style={{
              width: "80px",
              height: "auto",
              maxHeight: "80px",
              cursor: "pointer",
              border: "1px solid #ccc",
              borderRadius: "4px",
            }}
            onError={(e) => (e.currentTarget.style.display = "none")} // Hide if image fails to load
          />
        </a>
      );
    },
  },
  {
    accessorKey: "processedImageUrl",
    header: "Processed Image",
    cell: ({ row }) => {
      const imageUrl = row.getValue("processedImageUrl") as string;
      if (!imageUrl) return "N/A";
      return (
        <a href={imageUrl} target="_blank" rel="noopener noreferrer">
          <img
            src={imageUrl}
            alt="Processed Prediction"
            style={{
              width: "80px",
              height: "auto",
              maxHeight: "80px",
              cursor: "pointer",
              border: "1px solid #ccc",
              borderRadius: "4px",
            }}
            onError={(e) => (e.currentTarget.style.display = "none")} // Hide if image fails to load
          />
        </a>
      );
    },
  },
];
