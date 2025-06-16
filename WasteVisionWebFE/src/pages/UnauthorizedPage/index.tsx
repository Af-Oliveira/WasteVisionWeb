import { Button } from "@/components/ui/button";
import { Link } from "react-router-dom";

export const UnauthorizedPage = () => {
  return (
    <div className="min-h-[80vh] flex items-center justify-center bg-background">
      <div className="text-center space-y-6">
        <div className="space-y-2">
          <h1 className="text-7xl font-bold text-destructive">401</h1>
          <h2 className="text-3xl font-semibold text-foreground">
            Unauthorized Access
          </h2>
          <p className="text-muted-foreground">
            You don't have permission to access this page.
          </p>
        </div>
        <div className="flex gap-4 justify-center">
          <Button asChild variant="default" size="lg">
            <Link to="/">Return Home</Link>
          </Button>
          <Button asChild variant="outline" size="lg">
            <Link to="/logout">Log in</Link>
          </Button>
        </div>
      </div>
    </div>
  );
};
