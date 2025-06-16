import { useAuthService } from "@/di/container";
import React, { createContext, useEffect, useState } from "react";
import { UserInfo } from "../core/domain/Auth";
import { IAuthService } from "@/core/services/interfaces/auth-service";
import { ApiError } from "@/data/http/httpClient";
import { useToast } from "@/hooks/useToast";

interface AuthContextType {
  user: UserInfo | null;
  loading: boolean;
  login: () => void;
  logout: () => Promise<void>;
}

export const AuthContext = createContext<AuthContextType | undefined>(
  undefined
);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const { error } = useToast();
  const [user, setUser] = useState<UserInfo | null>(null);
  const [loading, setLoading] = useState(true);
  const authService: IAuthService = useAuthService();

  const fetchUserInfo = async () => {
    try {
      setLoading(true);
      const userInfo = await authService.getUserInfo();
      setUser(userInfo);
    } catch (err) {
      setUser(null);
      if (err instanceof ApiError) {
        error(err.message);
      } else {
        error("Failed to fetch user info");
      }
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUserInfo();
  }, []);

  const login = () => {
    authService.login();
  };

  const logout = async () => {
    try {
      setLoading(true);
      await authService.logout();
      setUser(null);
    } catch (err) {
      if (err instanceof ApiError) {
        error(err.message);
      } else {
        error("Failed to logout");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <AuthContext.Provider value={{ user, loading, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
