import { defineConfig } from "#q-app/wrappers";
import path from "path";
import { env } from "process";
import { nodePolyfills } from "vite-plugin-node-polyfills";

export default defineConfig(() => {
  const target = "https://localhost:5001";

  const base = env.VITE_ROUTER_BASE;
  const basePath = base ?? "";

  const proxyRoutes = ["/api"];

  const proxy: Record<string, any> | undefined = {};
  proxyRoutes.forEach((route) => {
    proxy[`^${basePath}${route}`] = { target, secure: false };
  });

  return {
    extras: ["roboto-font", "material-icons"],
    build: {
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
