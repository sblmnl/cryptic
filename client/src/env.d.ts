declare namespace NodeJS {
  interface ProcessEnv {
    NODE_ENV: string;
    VITE_APP_NAME?: string;
    VITE_CLIENT_NAME?: string;
    VITE_CLIENT_VERSION?: string;
    VITE_ROUTER_MODE?: "hash" | "history" | "abstract";
    VITE_ROUTER_BASE?: string;
  }
}

declare module "argon2-browser/dist/argon2-bundled.min.js" {
  export * from "argon2-browser";
}

type Guid = string;
