import { Link } from "react-router-dom";
import { cn } from "@/lib/utils";

export function Footer() {
  return (
    <footer className="w-full border-t bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="container mx-auto px-6 py-8">
        <div className="flex flex-col md:flex-row justify-between items-center space-y-6 md:space-y-0">
          <div className="text-base font-medium text-muted-foreground/80">
            © {new Date().getFullYear()}{" "}
            <span className="text-primary font-semibold">WasteVision AI</span>
            <span className="text-muted-foreground/70"> · All rights reserved.</span>
          </div>
          
          <nav className="flex items-center space-x-8">
            <FooterLink to="/privacy-policy">Privacy Policy</FooterLink>
            <FooterLink to="/data-protection">Data Protection</FooterLink>
            <FooterLink 
              href="mailto:contact@wastevision.ai" 
              external
            >
              Contact Us
            </FooterLink>
          </nav>
        </div>
      </div>
    </footer>
  );
}

interface FooterLinkProps extends React.ComponentPropsWithoutRef<"a"> {
  to?: string;
  external?: boolean;
  children: React.ReactNode;
}

function FooterLink({ 
  to, 
  external,
  className,
  children,
  ...props 
}: FooterLinkProps) {
  const styles = cn(
    "text-sm font-medium text-muted-foreground/80 transition-colors",
    "hover:text-primary",
    "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring",
    className
  );

  if (external) {
    return (
      <a 
        className={styles}
        target="_blank"
        rel="noopener noreferrer"
        {...props}
      >
        {children}
      </a>
    );
  }

  return (
    <Link 
      to={to!}
      className={styles}
    >
      {children}
    </Link>
  );
}