declare namespace NodeJS {
  interface ProcessEnv {
    NODE_ENV: string;
    VITE_APP_NAME?: string;
    VITE_CLIENT_NAME?: string;
    VITE_CLIENT_VERSION?: string;
    VITE_ROUTER_MODE?: "hash" | "history" | "abstract";
    VITE_ROUTER_BASE?: string;
    VITE_GITHUB_REPO_URL?: string;
    VITE_DONATE_KOFI_URL?: string;
    VITE_DONATE_MONERO_ADDRESS?: string;
  }
}

type Guid = string;
