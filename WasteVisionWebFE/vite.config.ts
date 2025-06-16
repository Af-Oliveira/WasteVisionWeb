import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import path from "path";
import { fileURLToPath } from "url";
import basicSsl from "@vitejs/plugin-basic-ssl";

const __dirname = path.dirname(fileURLToPath(import.meta.url));

export default defineConfig({
  plugins: [react() /*basicSsl()*/],
  base: "/",
  server: {
    port: 5173,
    proxy: {
      "/api": {
        target: "http://localhost:3000",
        changeOrigin: true,
        secure: false,
        ws: true,
        configure: (proxy, _options) => {
          proxy.on("proxyReq", (proxyReq, req, _res) => {
            // Copy cookies from the incoming request
            const cookies = req.headers.cookie;
            if (cookies) {
              proxyReq.setHeader("cookie", cookies);
            }
            console.log("Proxy Request Headers:", proxyReq.getHeaders());
          });
        },
      },
    },
  },
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
});
