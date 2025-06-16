import { useAuth } from "@/hooks/useAuth";
import React from "react";
import { Navigate, useLocation } from "react-router-dom";
import { LoadingSpinner } from "./ui/loading-spinner";

interface ProtectedRouteProps {
  children: React.ReactNode;
  allowedRoles?: string[];
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({
  children,
  allowedRoles,
}) => {
  const { user, loading } = useAuth();
  const location = useLocation();

  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen">
        <LoadingSpinner />
      </div>
    );
  }

  if (!user) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (
    allowedRoles &&
    user.role?.description &&
    !allowedRoles.includes(user.role.description)
  ) {
    return <Navigate to="/unauthorized" replace />;
  }

  return <>{children}</>;
};
