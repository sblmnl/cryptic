import { defineConfig } from "#q-app/wrappers";
import dotenv from "dotenv";
import path from "path";
import { nodePolyfills } from "vite-plugin-node-polyfills";

dotenv.config();

export default defineConfig(() => {
  const target = "https://localhost:5001";

  const base = process.env.VITE_ROUTER_BASE;
  const basePath = base ?? "";

  const proxyRoutes = ["/api"];

  const proxy: Record<string, any> | undefined = {};
  proxyRoutes.forEach((route) => {
    proxy[`^${basePath}${route}`] = { target, secure: false };
  });

  return {
    boot: ["quasar-defaults", "dark-mode", "axios"],
    extras: ["eva-icons", "fontawesome-v6"],
    framework: {
      plugins: ["Notify", "Dialog", "Loading", "Dark"],
      config: {
        notify: {},
        dark: "auto",
      },
      iconSet: "eva-icons",
    },
    css: ["typography.scss", "app.scss"],
    build: {
      publicPath: base ? `${base}/` : "/",
      env: {
        VITE_APP_NAME: process.env.VITE_APP_NAME,
        VITE_CLIENT_NAME: process.env.VITE_CLIENT_NAME,
        VITE_CLIENT_VERSION: process.env.VITE_CLIENT_VERSION,
        VITE_ROUTER_MODE: process.env.VITE_ROUTER_MODE,
        VITE_ROUTER_BASE: process.env.VITE_ROUTER_BASE,
        VITE_GITHUB_REPO_URL: process.env.VITE_GITHUB_REPO_URL,
        VITE_DONATE_KOFI_URL: process.env.VITE_DONATE_KOFI_URL,
        VITE_DONATE_MONERO_ADDRESS: process.env.VITE_DONATE_MONERO_ADDRESS,
      },
      target: {
        browser: ["es2022", "firefox115", "chrome115", "safari14"],
        node: "node20",
      },
      alias: {
        "@": path.resolve(__dirname, "./src"),
      },
      typescript: {
        strict: true,
        vueShim: true,
      },
      vueRouterMode: "hash",
      vitePlugins: [
        [
          "vite-plugin-checker",
          {
            vueTsc: true,
            eslint: {
              lintCommand: 'eslint -c ./eslint.config.js "./src*/**/*.{ts,js,mjs,cjs,vue}"',
              useFlatConfig: true,
            },
          },
          { server: false },
        ],
        nodePolyfills({
          globals: {
            Buffer: true,
            global: true,
            process: true,
          },
        }),
      ],
    },
    devServer: {
      port: 5173,
      https: true,
      proxy,
      open: false,
    },
  };
});
