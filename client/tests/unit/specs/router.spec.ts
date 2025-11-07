import { getAppRoute } from "@/router";

describe("router.ts", () => {
  describe("getAppRoute()", () => {
    it("returns the input route if no app path base is set", () => {
      const pathBases = [undefined, ""];
      const inputRoute = "/some-path";

      const appRoutes = pathBases.map((x) => {
        vi.stubEnv("VITE_APP_PATH_BASE", x);
        return getAppRoute(inputRoute);
      });

      expect(appRoutes).toStrictEqual([inputRoute, inputRoute]);
    });

    it("returns the input route with the app path base prepended", () => {
      vi.stubEnv("VITE_APP_PATH_BASE", "/my-app");
      const pathBase = import.meta.env.VITE_APP_PATH_BASE;
      const inputRoute = "/some-path";
      const appRoute = getAppRoute(inputRoute);

      expect(appRoute).toStrictEqual(`${pathBase}${inputRoute}`);
    });
  });
});
