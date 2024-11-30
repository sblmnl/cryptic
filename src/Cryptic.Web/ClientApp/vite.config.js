import fs from "node:fs";
import { env } from "node:process";
import vue from "@vitejs/plugin-vue";
import { defineConfig } from "vite";

const target = env.ASPNETCORE_HTTPS_PORT
  ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
  : env.ASPNETCORE_URLS
    ? env.ASPNETCORE_URLS.split(";")[0]
    : "http://localhost:5000";

const useHttps = target.startsWith("https");

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    port: 3000,
    proxy: {
      "^/api": {
        target,
        secure: false,
      },
    },
    https: useHttps && {
      key: fs.readFileSync("../../../.dev/ssl/localhost.key"),
      cert: fs.readFileSync("../../../.dev/ssl/localhost.crt"),
    },
  },
  build: {
    outDir: "../wwwroot",
    emptyOutDir: true,
  },
});
