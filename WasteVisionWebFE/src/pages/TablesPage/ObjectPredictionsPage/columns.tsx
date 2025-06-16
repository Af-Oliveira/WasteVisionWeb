// src/features/predictions/components/columns.tsx
import { ObjectPrediction } from "@/core/domain/ObjectPrediction";
import { ColumnDef, Row } from "@tanstack/react-table";

export const columns: ColumnDef<ObjectPrediction>[] = [

  {
    accessorKey: "x",
    header: "X coordinate",
  },
  {
    accessorKey: "y",
    header: "Y coordinate",
  },
  {
    accessorKey: "width",
    header: "Width",
  },
  {
    accessorKey: "height",
    header: "Height",
  },
  {
    accessorKey: "category",
    header: "Category",
  },
  {
    accessorKey: "confidence",
    header: "Confidence (%)",
  },
  {
    accessorKey: "predictionId",
    header: "Prediction ID",
  },
];
