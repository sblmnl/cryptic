import { expect, test } from "@playwright/test";

test("home page redirects to /notes", async ({ page }) => {
  await page.goto("/");
  await expect(page).toHaveURL(/\/notes$/);
});
