import { defineBoot } from "#q-app/wrappers";
import axios from "axios";
import { getAppRoute } from "../router";

const api = axios.create({
  baseURL: getAppRoute("/api"),
  headers: {
    "Content-Type": "application/json",
  },
});

export default defineBoot(({ app }) => {
  app.config.globalProperties.$axios = axios;
  app.config.globalProperties.$api = api;
});

export { api, axios };
