import { AuthProvider } from "./context/AuthContext";
import { BrowserRouter } from "react-router-dom";
import { ThemeProvider } from "./components/theme-provider";
import { Router } from "./routes/Router";
import { ToastProvider } from "./providers/ToastProvider";
import { DIProvider } from "./di/container";

export function App() {
  return (
    <DIProvider>
      <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
        <ToastProvider>
          <AuthProvider>
            <BrowserRouter>
              <Router />
            </BrowserRouter>
          </AuthProvider>
        </ToastProvider>
      </ThemeProvider>
    </DIProvider>
  );
}
