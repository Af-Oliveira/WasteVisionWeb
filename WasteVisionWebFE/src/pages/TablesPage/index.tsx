import { TABLE_COMPONENTS, type AvailableTables } from "./config";

export interface TablesPageParams {
  table: AvailableTables;
}

export function TablesPage({ table }: TablesPageParams) {
  const TableComponent = TABLE_COMPONENTS[table];

  return (
    <div className="container mx-auto py-6">
      <TableComponent />
    </div>
  );
}
