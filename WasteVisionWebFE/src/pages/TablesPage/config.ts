import { ObjectPredictionPage } from "./ObjectPredictionsPage";
import { PredictionsPage } from "./PredictionPage";
import { RoboflowModelsPage } from "./RoboflowModelsPage";
import { RolesPage } from "./RolesPage";
import { UsersPage } from "./UsersPage";

export type AvailableTables = "users" | "roles" | "roboflowmodels" | "predictions" | "objectpredictions";
type TableComponent = React.ComponentType<{}>;

export const TABLE_COMPONENTS: Record<AvailableTables, TableComponent> = {
  users: UsersPage,
  roles: RolesPage,
  roboflowmodels: RoboflowModelsPage,
  predictions: PredictionsPage,
  objectpredictions: ObjectPredictionPage
} as const;
