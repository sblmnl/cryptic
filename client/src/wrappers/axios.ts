import { getAppRoute } from "@/router";
import axios from "axios";

const api = axios.create({
  baseURL: getAppRoute("/api"),
  headers: {
    "Content-Type": "application/json",
  },
});

export { api };
