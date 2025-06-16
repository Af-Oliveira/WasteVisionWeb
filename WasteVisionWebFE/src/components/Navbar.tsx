import { Button, buttonVariants } from "./ui/button";
import { Link } from "react-router-dom";
import { Sheet, SheetContent, SheetTrigger } from "./ui/sheet";
import { ModeToggle } from "./mode-toggle";
import { useAppNavbar } from "@/hooks/useAppNavbar";
import { cn } from "@/lib/utils";

export function Navbar() {
  const { config } = useAppNavbar();

  return (
    <nav className="sticky top-0 z-50 w-full border-b border-border/40 bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60 shadow-sm">
      <div className="container flex h-16 max-w-screen-2xl items-center">
        <div className="flex flex-1 items-center justify-between px-4">
          {/* Brand Logo/Name - Enhanced */}
          <Link
            to={config.brandPath}
            className={cn(
              buttonVariants({ variant: "link" }),
              "font-bold text-2xl tracking-tight hover:text-primary transition-colors"
            )}
          >
            {config.brandName}
          </Link>

          {/* Desktop Navigation - Enhanced */}
          <div className="hidden md:flex items-center space-x-4">
            {config.items.map((item) => (
              <Link
                key={item.path}
                to={item.path}
                className={cn(
                  buttonVariants({ variant: "ghost" }),
                  "text-base font-medium hover:text-primary hover:bg-primary/10 transition-all duration-200"
                )}
              >
                {item.label}
              </Link>
            ))}
            <div className="ml-4 pl-4 border-l border-border/60">
              <ModeToggle />
            </div>
          </div>

          {/* Mobile Navigation - Enhanced */}
          <Sheet>
            <SheetTrigger asChild>
              <Button
                variant="ghost"
                size="icon"
                className="md:hidden hover:bg-primary/10 transition-colors"
                aria-label="Open Menu"
              >
                <MenuIcon className="h-6 w-6" />
              </Button>
            </SheetTrigger>
            <SheetContent 
              side="right" 
              className="w-[80%] max-w-sm border-l border-border/40 backdrop-blur-lg"
            >
              <div className="flex flex-col gap-6 py-6">
                {config.items.map((item) => (
                  <Link
                    key={item.path}
                    to={item.path}
                    className={cn(
                      buttonVariants({ variant: "ghost" }),
                      "text-lg font-medium justify-start hover:text-primary hover:bg-primary/10 transition-all duration-200"
                    )}
                  >
                    {item.label}
                  </Link>
                ))}
                <div className="mt-4 pt-4 border-t border-border/60">
                  <ModeToggle />
                </div>
              </div>
            </SheetContent>
          </Sheet>
        </div>
      </div>
    </nav>
  );
}

// Enhanced MenuIcon with better stroke width
function MenuIcon(props: React.SVGProps<SVGSVGElement>) {
  return (
    <svg
      {...props}
      xmlns="http://www.w3.org/2000/svg"
      width="24"
      height="24"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="1.75"
      strokeLinecap="round"
      strokeLinejoin="round"
    >
      <line x1="4" x2="20" y1="12" y2="12" />
      <line x1="4" x2="20" y1="6" y2="6" />
      <line x1="4" x2="20" y1="18" y2="18" />
    </svg>
  );
}
