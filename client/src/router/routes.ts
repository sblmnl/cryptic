import type { RouteRecordRaw } from "vue-router";

const routes: RouteRecordRaw[] = [
  {
    alias: "/",
    path: "/notes",
    component: () => import("@/pages/notes/CreateNotePage.vue"),
  },
  {
    path: "/notes/:noteId",
    component: () => import("@/pages/notes/ReadNotePage.vue"),
  },
  {
    path: "/contribute",
    component: () => import("@/pages/ContributePage.vue"),
  },
  {
    path: "/:catchAll(.*)",
    component: () => import("@/pages/NotFoundPage.vue"),
  },
];

export default routes;
