import type { RouteRecordRaw } from "vue-router";

const routes: RouteRecordRaw[] = [
  {
    path: "/",
    component: () => import("@/pages/HomePage.vue"),
  },
  {
    path: "/notes",
    component: () => import("@/pages/notes/CreateNotePage.vue"),
  },
  {
    path: "/notes/:noteId",
    component: () => import("@/pages/notes/ReadNotePage.vue"),
  },
  {
    path: "/:catchAll(.*)",
    component: () => import("@/pages/NotFoundPage.vue"),
  },
];

export default routes;
