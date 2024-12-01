import { createVuetify } from "vuetify";
import "@mdi/font/css/materialdesignicons.css";
import "vuetify/styles";

const githubDarkTheme = {
  dark: true,
  colors: {
    background: "#0d1117",
    surface: "#0d1117",
    primary: "#58a6ff",
    "primary-darken-1": "#427dc0",
    secondary: "#ff7b72",
    "secondary-darken-1": "#c05d56",
    error: "#ff7b72",
    info: "#58a6ff",
    success: "#3fb950",
    warning: "#d29922",
  },
};

export default createVuetify({
  theme: {
    defaultTheme: "githubDarkTheme",
    themes: {
      githubDarkTheme,
    },
  },
});
