import App from "@/App.vue";
import pinia from "@/core/stores";
import router from "@/router";
import { createApp } from "vue";

import "@/core/styles/main.css";

const app = createApp(App);

app.use(router);
app.use(pinia);

app.mount("#app");
