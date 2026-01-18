import { AppLayout } from "@/layouts";
import { createRouter, createWebHashHistory, createWebHistory } from "vue-router";
import routes from "./routes";

declare module "vue-router" {
  interface RouteMeta {
    title?: string;
    layout?: AppLayout;
  }
}

export function getAppRoute(rootRelativeRoute: string) {
  return import.meta.env.VITE_ROUTER_BASE
    ? `${import.meta.env.VITE_ROUTER_BASE}${rootRelativeRoute}`
    : rootRelativeRoute;
}

export const appBaseUrl = `${window.location.protocol}//${window.location.host}${getAppRoute("")}`;

const createHistory = import.meta.env.VITE_ROUTER_MODE === "history" ? createWebHistory : createWebHashHistory;

const router = createRouter({
  scrollBehavior: () => ({ left: 0, top: 0 }),
  routes,
  history: createHistory(import.meta.env.VITE_ROUTER_BASE),
});

router.beforeEach((to, _from, next) => {
  to.meta.layout ??= AppLayout.Default;
  window.document.title = to.meta.title ?? import.meta.env.VITE_APP_NAME;

  return next();
});

export default router;
