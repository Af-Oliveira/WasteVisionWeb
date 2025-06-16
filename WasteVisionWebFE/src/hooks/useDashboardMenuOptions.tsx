import { Role } from "@/@types/roles";
import {
  Home,
  LayoutDashboard,
  LogOut,
  LucideIcon,
  ScrollText,
  Shield,
  Users,
} from "lucide-react";
import { useAuth } from "./useAuth";

interface MenuOption {
  label: string;
  path: string;
  icon: LucideIcon;
}

interface MenuGroup {
  label: string;
  options: MenuOption[];
}

export const useDashboardMenuOptions = () => {
  const { user } = useAuth();

  // Common options that appear for all authenticated users
  const commonMenuGroups: MenuGroup[] = [
    {
      label: "Navigation",
      options: [
        {
          label: "Home",
          path: "/",
          icon: Home,
        },
        {
          label: "Dashboard",
          path: "/dashboard",
          icon: LayoutDashboard,
        },
      ],
    },
  ];

  // Footer options that appear for all users at the bottom
  const footerMenuGroup: MenuGroup = {
    label: "Account",
    options: [
      {
        label: "Logout",
        path: "/logout",
        icon: LogOut,
      },
    ],
  };

  // Role-specific menu options
  const roleMenuOptions: Record<Role, MenuGroup[]> = {
    User: [],
    Admin: [
      {
        label: "Management",
        options: [
          {
            label: "Users",
            path: "/dashboard/users",
            icon: Users,
          },
          {
            label: "Roles",
            path: "/dashboard/roles",
            icon: Shield,
          },
          {
            label: "RoboflowModels",
            path: "/dashboard/roboflowmodels",
            icon: Users,
          },
           {
            label: "Predictions",
            path: "/dashboard/predictions",
            icon: Users,
          },
          {
            label: "Object Predictions",
            path: "/dashboard/objectpredictions",
            icon: Users,
          },
        ],
      },
    ],
  };

  return {
    menuGroups: user?.role
      ? [...commonMenuGroups, ...roleMenuOptions[user.role.description as Role]]
      : commonMenuGroups,
    footerGroup: footerMenuGroup,
  };
};
