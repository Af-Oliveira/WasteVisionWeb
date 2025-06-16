import { Button } from "@/components/ui/button";
import { Link } from "react-router-dom";

export const NotFoundPage = () => {
  return (
    <div className="min-h-[80vh] flex items-center justify-center bg-background">
      <div className="text-center space-y-6">
        <div className="space-y-2">
          <h1 className="text-7xl font-bold text-primary">404</h1>
          <h2 className="text-3xl font-semibold text-foreground">
            Page Not Found
          </h2>
          <p className="text-muted-foreground">
            The page you're looking for doesn't exist or has been moved.
          </p>
        </div>
        <Button asChild variant="default" size="lg">
          <Link to="/">Return Home</Link>
        </Button>
      </div>
    </div>
  );
};
