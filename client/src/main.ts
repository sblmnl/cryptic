import App from "@/App.vue";
import pinia from "@/core/stores";
import router from "@/router";
import { createApp } from "vue";

import "bootstrap/dist/css/bootstrap-grid.min.css";
import "bootstrap/dist/css/bootstrap-utilities.min.css";
import vuetify from "./vuetify";

import "@/core/styles/main.css";

const app = createApp(App);

app.use(router);
app.use(pinia);
app.use(vuetify);

app.mount("#app");
