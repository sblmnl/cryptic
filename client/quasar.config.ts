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
    extras: ["roboto-font", "material-icons"],
    build: {
      env: {
        VITE_APP_NAME: process.env.VITE_APP_NAME,
        VITE_CLIENT_NAME: process.env.VITE_CLIENT_NAME,
        VITE_CLIENT_VERSION: process.env.VITE_CLIENT_VERSION,
        VITE_ROUTER_MODE: process.env.VITE_ROUTER_MODE,
        VITE_ROUTER_BASE: process.env.VITE_ROUTER_BASE,
      },
      publicPath: base ? `${base}/` : "/",
      target: {
        browser: ["es2022", "firefox115", "chrome115", "safari14"],
        node: "node20",
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
      alias: {
        "@": path.resolve(__dirname, "./src"),
      },
    },
    framework: {
      plugins: ["Notify", "Dialog", "Loading"],
      config: {
        notify: {},
        dark: "auto",
      },
    },
    devServer: {
      port: 5173,
      https: true,
      proxy,
      open: false,
    },
  };
});
