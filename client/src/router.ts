import { AppLayout } from "@/core/enums/ui";
import { createRouter, createWebHistory } from "vue-router";

declare module "vue-router" {
  interface RouteMeta {
    title?: string;
    layout?: AppLayout;
  }
}

export const appBaseUrl = window.location.protocol + "//" + window.location.host;

export function getAppRoute(rootRelativeRoute: string) {
  const pathBase = import.meta.env.VITE_APP_PATH_BASE;

  if (!pathBase) {
    return rootRelativeRoute;
  }

  return `${pathBase}${rootRelativeRoute}`;
}

const router = createRouter({
  history: createWebHistory(import.meta.env.VITE_APP_PATH_BASE),
  routes: [
    {
      path: "/",
      component: () => import("@/core/pages/HomePage.vue"),
    },
    {
      path: "/notes",
      component: () => import("@/modules/notes/pages/CreateNotePage.vue"),
    },
    {
      path: "/notes/:noteId",
      component: () => import("@/modules/notes/pages/ReadNotePage.vue"),
    },
    {
      path: "/:catchAll(.*)",
      component: () => import("@/core/pages/NotFoundPage.vue"),
    },
  ],
});

router.beforeEach((to, _from, next) => {
  to.meta.layout ??= AppLayout.Default;
  window.document.title = to.meta.title ?? "Cryptic";

  return next();
});

export default router;
