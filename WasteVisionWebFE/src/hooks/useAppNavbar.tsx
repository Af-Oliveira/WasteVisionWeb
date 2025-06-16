import { useAuth } from "./useAuth";
import { Role } from "@/@types/roles";

interface NavItem {
  label: string;
  path: string;
}

interface NavbarConfig {
  brandName: string;
  brandPath: string;
  items: NavItem[];
}

export const useAppNavbar = () => {
  const { user } = useAuth();

  const roleBasedItems: Record<Role, NavItem[]> = {
    Admin: [{ label: "Dashboard", path: "/dashboard" }],
    User: [],
  };

  const commonAuthenticatedItems: NavItem[] = [
    { label: "Home", path: "/" },
    { label: "Logout", path: "/logout" },
    { label: "Profile", path: "/me" },
    { label: "Scan", path: "/scan" },
  ];

  const nonAuthenticatedItems: NavItem[] = [
    { label: "Home", path: "/" },
    { label: "Login", path: "/login" },
  ];

  const config: NavbarConfig = {
    brandName: "AI Power Vision",
    brandPath: "/",
    items: user?.role
      ? [
          ...commonAuthenticatedItems,
          ...roleBasedItems[user.role.description as Role],
        ]
      : nonAuthenticatedItems,
  };

  return {
    config,
    isAuthenticated: !!user,
  };
};
