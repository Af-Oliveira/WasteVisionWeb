import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/components/ui/sidebar";
import { useAuth } from "@/hooks/useAuth";
import { useDashboardMenuOptions } from "@/hooks/useDashboardMenuOptions";
import { cn } from "@/lib/utils";
import {
  HelpCircle,
  Leaf,
  LogOut,
  Moon,
  ShieldCheck,
  Sun,
  User,
} from "lucide-react";
import { Link, useLocation } from "react-router-dom";
import { useTheme } from "@/components/theme-provider";

export function AppSidebar() {
  const { theme, setTheme } = useTheme();
  const { menuGroups, footerGroup } = useDashboardMenuOptions();
  const location = useLocation();
  const { user } = useAuth();

  const getUserInitials = () => {
    if (!user) return "MG";
    const username = user.username || "";

    return `${username.charAt(0)}`.toUpperCase() || "MG";
  };

  const getRoleBadgeColor = () => {
    if (!user?.role)
      return "bg-gray-200 text-gray-700 dark:bg-gray-700 dark:text-gray-300";

    switch (user.role.description) {
      case "Admin":
        return "bg-amber-100 text-amber-800 border-amber-300 dark:bg-amber-900/30 dark:text-amber-300 dark:border-amber-700";
      case "User":
        return "bg-green-100 text-green-800 border-green-300 dark:bg-green-900/30 dark:text-green-300 dark:border-green-700";
      default:
        return "bg-gray-100 text-gray-800 border-gray-300 dark:bg-gray-800 dark:text-gray-300 dark:border-gray-700";
    }
  };

  const getRoleIcon = () => {
    if (!user?.role) return null;
    switch (user.role.description) {
      case "Admin":
        return <ShieldCheck className="h-3 w-3" />;
      case "User":
        return <User className="h-3 w-3" />;
      default:
        return null;
    }
  };

  // Toggle between light and dark theme
  const toggleTheme = () => {
    setTheme(theme === "dark" ? "light" : "dark");
  };

  return (
    <Sidebar className="border-r border-border bg-sidebar-background">
      <SidebarHeader className="border-b border-border pb-4">
        <div className="flex items-center justify-between px-4 pt-4 mb-4">
          <div className="flex items-center gap-2">
            <Leaf className="h-6 w-6 text-primary" />
            <span className="text-lg font-semibold text-sidebar-foreground">
              WasteVision
            </span>
          </div>

          {/* Simple Theme Toggle */}
          <button
            onClick={toggleTheme}
            className="p-2 rounded-md text-sidebar-foreground/80 hover:text-primary bg-sidebar-accent/50 hover:bg-sidebar-accent transition-all duration-200"
            aria-label={`Switch to ${
              theme === "dark" ? "light" : "dark"
            } theme`}
            title={`Switch to ${theme === "dark" ? "light" : "dark"} theme`}
          >
            {theme === "dark" ? (
              <Sun className="h-4 w-4" />
            ) : (
              <Moon className="h-4 w-4" />
            )}
          </button>
        </div>

        {/* User Profile Section */}
        <div className="mx-4 p-3 rounded-lg bg-sidebar-accent/50 border border-border transition-all duration-200 hover:bg-sidebar-accent">
          <div className="flex items-center gap-3">
            <Avatar className="h-12 w-12 border-2 border-primary/20 ring-2 ring-primary/10">
              <AvatarImage src={""} alt={user?.username || "User"} />
              <AvatarFallback className="bg-primary text-primary-foreground">
                {getUserInitials()}
              </AvatarFallback>
            </Avatar>

            <div className="flex-1 min-w-0">
              <h3 className="font-medium text-sidebar-foreground truncate">
                {user?.username || "Gardener"}
              </h3>
              <div className="flex items-center gap-1.5 mt-1">
                <span
                  className={cn(
                    "text-xs px-2 py-0.5 rounded-full border flex items-center gap-1",
                    getRoleBadgeColor()
                  )}
                >
                  {getRoleIcon()}
                  {user?.role?.description || "Guest"}
                </span>
              </div>
            </div>
          </div>
        </div>

        {/* Improved Logout Button - separate from user card */}
        <div className="mx-4 mt-3 mb-1">
          <Link
            to="/logout"
            className="flex items-center justify-center gap-2 py-2 px-4 w-full rounded-md border border-border/80 text-sidebar-foreground/80 hover:text-destructive hover:border-destructive/30 hover:bg-destructive/5 transition-all duration-200 group"
            title="Logout from your account"
          >
            <LogOut className="h-4 w-4 group-hover:scale-110 transition-transform" />
            <span className="text-sm font-medium">Sign Out</span>
          </Link>
        </div>
      </SidebarHeader>

      <SidebarContent className="py-4">
        {/* Menu Groups */}
        {menuGroups.map((group) => (
          <SidebarGroup key={group.label} className="mb-4">
            <SidebarGroupLabel className="text-xs font-medium text-muted-foreground px-6 mb-2 uppercase tracking-wider">
              {group.label}
            </SidebarGroupLabel>
            <SidebarGroupContent>
              <SidebarMenu>
                {group.options.map((item) => (
                  <SidebarMenuItem key={item.path}>
                    <SidebarMenuButton
                      asChild
                      isActive={location.pathname === item.path}
                      tooltip={item.label}
                      className={cn(
                        "transition-all duration-200 rounded-md mx-3 px-3",
                        location.pathname === item.path
                          ? "bg-primary/10 text-primary font-medium dark:bg-primary/20"
                          : "hover:bg-sidebar-accent text-sidebar-foreground/80 hover:text-sidebar-foreground"
                      )}
                    >
                      <Link
                        to={item.path}
                        className="flex items-center gap-3 py-2"
                      >
                        <item.icon
                          className={cn(
                            "h-5 w-5",
                            location.pathname === item.path
                              ? "text-primary"
                              : "text-sidebar-foreground/60"
                          )}
                        />
                        <span>{item.label}</span>

                        {/* Show indicator for active item */}
                        {location.pathname === item.path && (
                          <div className="w-1.5 h-1.5 rounded-full bg-primary ml-auto" />
                        )}
                      </Link>
                    </SidebarMenuButton>
                  </SidebarMenuItem>
                ))}
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
        ))}
      </SidebarContent>

      {/* Footer Group - simplified */}
      <SidebarFooter className="border-t border-border pt-4 mt-auto">
        {/* Help Section */}
        <div className="mx-4 p-3 rounded-lg bg-primary/5 border border-primary/20 flex items-center gap-2 transition-all duration-200 hover:bg-primary/10">
          <HelpCircle className="h-5 w-5 text-primary flex-shrink-0" />
          <span className="text-xs text-sidebar-foreground">
            Need help with the platform? Visit our{" "}
            <Link
              to="/help"
              className="underline font-medium text-primary hover:text-primary/80"
            >
              Help Center
            </Link>
          </span>
        </div>
      </SidebarFooter>
    </Sidebar>
  );
}
