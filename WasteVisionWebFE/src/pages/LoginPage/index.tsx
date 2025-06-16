import React from "react";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter, // Added for potential future use or spacing
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { LoadingSpinner } from "@/components/ui/loading-spinner";
import { useAuth } from "@/hooks/useAuth";
import { Navigate, useLocation } from "react-router-dom";
import { LogIn, Leaf } from "lucide-react"; // Added LogIn and Leaf icons

export const LoginPage = () => {
  const { login, user, loading } = useAuth();
  const location = useLocation();

  // Enhanced full-page loading state
  if (loading && !user) {
    return (
      <div className="flex min-h-screen flex-col items-center justify-center bg-background text-foreground animated-subtle-background">
        <Leaf className="mb-4 h-16 w-16 animate-pulse text-primary" />
        <LoadingSpinner className="h-10 w-10 text-primary" />
        <p className="mt-6 text-lg text-muted-foreground">
          Signing in to WasteVision AI...
        </p>
      </div>
    );
  }

  if (user?.role?.description) {
    switch (user.role.description) {
      case "Admin":
        return <Navigate to="/dashboard" state={{ from: location }} replace />;
      case "User":
        return <Navigate to="/" state={{ from: location }} replace />;
      default: // Fallback for unknown roles, or redirect to a generic page
        return <Navigate to="/" state={{ from: location }} replace />;
    }
  }

  return (
    <div className="animated-subtle-background flex min-h-screen items-center justify-center bg-background p-4 md:p-6">
      <Card className="relative z-10 w-full max-w-md rounded-xl bg-card shadow-2xl ring-1 ring-border/20">
        <CardHeader className="space-y-3 pt-8 text-center">
          <div className="mb-4 flex justify-center">
            <Leaf className="h-14 w-14 text-primary" />
          </div>
          <CardTitle className="text-3xl font-bold tracking-tight text-foreground">
            Welcome to WasteVision AI
          </CardTitle>
          <CardDescription className="text-md px-4 text-muted-foreground">
            Sign in to unlock intelligent waste detection and contribute to a
            greener planet.
          </CardDescription>
        </CardHeader>
        <CardContent className="px-6 pb-8 pt-6 sm:px-8">
          <Button
            onClick={login}
            className="w-full transform rounded-lg px-8 py-3 text-lg font-semibold shadow-lg transition-all duration-300 ease-in-out hover:scale-[1.03] hover:shadow-primary/40 focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 focus-visible:ring-offset-background"
            size="lg"
            variant="default"
            disabled={loading}
          >
            {loading ? (
              <LoadingSpinner className="mr-2 h-5 w-5 animate-spin" />
            ) : (
              <LogIn className="mr-2 h-5 w-5" />
            )}
            {loading ? "Signing In..." : "Sign In"}
          </Button>
          <p className="mt-6 text-center text-xs text-muted-foreground">
            By signing in, you agree to our Terms of Service.
          </p>
        </CardContent>
      </Card>
    </div>
  );
};
