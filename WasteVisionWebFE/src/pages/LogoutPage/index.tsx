import React, { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { LoadingSpinner } from "@/components/ui/loading-spinner";
import { useAuth } from "@/hooks/useAuth";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/data/http/httpClient";

export const LogoutPage: React.FC = () => {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const { error } = useToast();

  useEffect(() => {
    const handleLogout = async () => {
      try {
        await logout();
        // The logout redirect is handled in the auth service
        // This is just a fallback
        navigate("/login");
      } catch (err) {
        if (err instanceof ApiError) {
          error(err.message);
        } else {
          error("Failed to logout. Please try again.");
        }
        navigate("/login");
      }
    };

    handleLogout();
  }, [logout, navigate, error]);

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8">
        <div className="flex flex-col items-center">
          <LoadingSpinner className="w-10 h-10" />
          <p className="mt-2 text-center text-sm text-gray-600">
            Please wait while we sign you out
          </p>
        </div>
      </div>
    </div>
  );
};
