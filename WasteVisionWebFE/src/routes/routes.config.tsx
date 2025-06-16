import { LandingPage } from "@/pages/LandingPage";
import { LoginPage } from "@/pages/LoginPage";
import { UnauthorizedPage } from "@/pages/UnauthorizedPage";
import { LogoutPage } from "@/pages/LogoutPage";
import { NotFoundPage } from "@/pages/NotFoundPage";
import { DashboardPage } from "@/pages/DashboardPage";
import { TablesPage } from "@/pages/TablesPage";
import { LayoutAdmin, LayoutApp, LayoutUser } from "./layouts";
import { Role } from "@/@types/roles";
import { PrivacyPolicyPage } from "@/pages/PrivacyPolicyPage";
import { DataProtectionPage } from "@/pages/DataProtectionPage";
import { ProtectedRoute } from "@/components/ProtectedRoute";
import { ScanPage } from "@/pages/ScanPage";
import { UserProfilePage } from "@/pages/ProfilePage";

interface Route {
  path: string;
  element: React.ReactNode;
  layout: React.ComponentType<{ children: React.ReactNode }>;
  roles?: Role[];
}

export const publicRoutes: readonly Route[] = [
  {
    path: "/",
    element: <LandingPage />,
    layout: LayoutApp,
  },
  {
    path: "/privacy-policy",
    element: <PrivacyPolicyPage />,
    layout: LayoutApp,
  },
  {
    path: "/data-protection",
    element: <DataProtectionPage />,
    layout: LayoutApp,
  },
  {
    path: "/login",
    element: <LoginPage />,
    layout: LayoutApp,
  },
  {
    path: "/unauthorized",
    element: <UnauthorizedPage />,
    layout: LayoutApp,
  },
  {
    path: "/logout",
    element: <LogoutPage />,
    layout: LayoutApp,
  },
  {
    path: "*",
    element: <NotFoundPage />,
    layout: LayoutApp,
  },
  {
    path: "/scan",
    element: <ScanPage />,
    layout: LayoutApp,
  },
    
] as const;

export const protectedRoutes: readonly Route[] = [
  {
    path: "/dashboard",
    element: <DashboardPage />,
    layout: LayoutAdmin,
    roles: ["Admin"],
  },
  {
    path: "/scan",
    element: <ScanPage />,
    layout: LayoutUser,
    roles: ["User"],
  },
  { 
    path: "/me",
    element: <UserProfilePage />,
    layout: LayoutUser,
    roles: ["User", "Admin"],
  },
  {
    path: "/dashboard/roles",
    element: <TablesPage table="roles" />,
    layout: LayoutAdmin,
    roles: ["Admin"],
  },
  {
    path: "/dashboard/users",
    element: <TablesPage table="users" />,
    layout: LayoutAdmin,
    roles: ["Admin"],
  },
   {
    path: "/dashboard/roboflowmodels",
    element: <TablesPage table="roboflowmodels" />,
    layout: LayoutAdmin,
    roles: ["Admin"],
  },
     {
    path: "/dashboard/predictions",
    element: <TablesPage table="predictions" />,
    layout: LayoutAdmin,
    roles: ["Admin"],
  },
  {
    path: "/dashboard/objectpredictions",
    element: <TablesPage table="objectpredictions" />,
    layout: LayoutAdmin,
    roles: ["Admin"],
  }
] as const;
