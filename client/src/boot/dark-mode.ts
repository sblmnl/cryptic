import { defineBoot } from "#q-app/wrappers";
import { Dark } from "quasar";

export default defineBoot(() => {
  const darkMode = localStorage.getItem("darkMode");

  if (darkMode == null) {
    return;
  }

  Dark.set(darkMode === "true");
});
