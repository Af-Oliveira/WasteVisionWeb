import { Navbar } from "@/components/Navbar";
import { Footer } from "@/components/Footer";
import { SidebarProvider, SidebarTrigger } from "@/components/ui/sidebar";
import { AppSidebar } from "@/components/app-sidebar";

interface LayoutProps {
  children: React.ReactNode;
}

export function LayoutApp({ children }: LayoutProps) {
  return (
    <div className="flex flex-col justify-between min-h-screen">
      <Navbar />
      {children}
      <Footer />
    </div>
  );
}

export function LayoutAdmin({ children }: LayoutProps) {
  return (
    <SidebarProvider>
      <AppSidebar />
      <SidebarTrigger />
      {children}
    </SidebarProvider>
  );
}

export function LayoutUser({ children }: LayoutProps) {
  return (
    <div className="flex flex-col justify-between min-h-screen">
      <Navbar />
      {children}
      <Footer />
    </div>
  );
}
