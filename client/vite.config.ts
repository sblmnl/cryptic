/// <reference types="vitest" />
import vue from "@vitejs/plugin-vue";
import child_process from "node:child_process";
import fs from "node:fs";
import path from "node:path";
import { env } from "node:process";
import { defineConfig, loadEnv, type ProxyOptions } from "vite";

const target = "https://localhost:5001";

const baseFolder =
  env.APPDATA !== undefined && env.APPDATA !== "" ? `${env.APPDATA}/ASP.NET/https` : `${env.HOME}/.aspnet/https`;

const certificateName = "localhost";
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync(baseFolder)) {
  fs.mkdirSync(baseFolder, { recursive: true });
}

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
  if (
    0 !==
    child_process.spawnSync(
      "dotnet",
      ["dev-certs", "https", "--export-path", certFilePath, "--format", "Pem", "--no-password"],
      { stdio: "inherit" },
    ).status
  ) {
    throw new Error("Could not create certificate.");
  }
}

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, __dirname, "");

  const base = env.VITE_APP_PATH_BASE;
  const basePath = base != null ? base : "";

  const proxyRoutes = ["/api"];

  const proxy: Record<string, string | ProxyOptions> | undefined = {};
  proxyRoutes.forEach((route) => {
    proxy[`^${basePath}${route}`] = { target, secure: false };
  });

  return {
    base,
    plugins: [vue()],
    resolve: {
      alias: {
        "@": path.resolve(__dirname, "./src"),
      },
      extensions: [".js", ".json", ".jsx", ".mjs", ".ts", ".tsx", ".vue"],
    },
    server: {
      proxy,
      https: {
        cert: fs.readFileSync(certFilePath),
        key: fs.readFileSync(keyFilePath),
      },
    },
    test: {
      environment: "jsdom",
      globals: true,
      setupFiles: ["./tests/unit/setup.ts"],
      unstubEnvs: true,
    },
  };
});
