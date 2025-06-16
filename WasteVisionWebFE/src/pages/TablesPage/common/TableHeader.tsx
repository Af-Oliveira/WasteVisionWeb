import { Button } from "@/components/ui/button";
import { Plus, PlusCircle, RefreshCw, Search } from "lucide-react";

interface TableHeaderProps {
  title: string;
  onRefresh: () => void;
  onCreate?: () => void;
  isLoading?: boolean;
  onSearch?: () => void;
}

export function TableHeader({
  title,
  onRefresh,
  onCreate,
  isLoading,
  onSearch,
}: TableHeaderProps) {
  return (
    <div className="flex justify-between items-center mb-6">
      <div className="space-y-1">
        <h2 className="text-2xl font-semibold tracking-tight">{title}</h2>
      </div>
      <div className="flex items-center gap-2">
        <Button
          variant="outline"
          size="icon"
          onClick={onRefresh}
          disabled={isLoading}
        >
          <RefreshCw className="h-4 w-4" />
        </Button>
        <div className="flex items-center gap-2">
          {onSearch && (
            <Button variant="outline" onClick={onSearch}>
              <Search className="h-4 w-4" />
              Search
            </Button>
          )}
          {onCreate && (
            <Button onClick={onCreate}>
              <Plus className="h-4 w-4 mr-2" />
              Create
            </Button>
          )}
        </div>
      </div>
    </div>
  );
}
