import { describe, expect, it, vi } from "vitest";
import { sleep } from "@/lib/util/time";

describe("sleep", () => {
  it("resolves after the specified duration", async () => {
    vi.useFakeTimers();

    let resolved = false;
    const promise = sleep(100).then(() => {
      resolved = true;
    });

    expect(resolved).toBe(false);

    await vi.advanceTimersByTimeAsync(100);
    await promise;

    expect(resolved).toBe(true);

    vi.useRealTimers();
  });
});
