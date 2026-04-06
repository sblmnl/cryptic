import { uint8ArrayToBase64Replacer } from "@/shared/util/json";
import { describe, expect, it } from "vitest";

describe("uint8ArrayToBase64Replacer", () => {
  it("passes through non-Uint8Array values unchanged", () => {
    expect(uint8ArrayToBase64Replacer("k", "hello")).toBe("hello");
    expect(uint8ArrayToBase64Replacer("k", 42)).toBe(42);
    expect(uint8ArrayToBase64Replacer("k", null)).toBe(null);
    expect(uint8ArrayToBase64Replacer("k", true)).toBe(true);
  });

  it("converts a Uint8Array to a base64 string", () => {
    const bytes = new Uint8Array([72, 101, 108, 108, 111]);
    expect(uint8ArrayToBase64Replacer("k", bytes)).toBe("SGVsbG8=");
  });

  it("converts an empty Uint8Array to an empty string", () => {
    expect(uint8ArrayToBase64Replacer("k", new Uint8Array([]))).toBe("");
  });

  it("works as a JSON.stringify replacer", () => {
    const obj = { data: new Uint8Array([1, 2, 3]), label: "test" };
    const json = JSON.parse(JSON.stringify(obj, uint8ArrayToBase64Replacer));
    expect(json.data).toBe("AQID");
    expect(json.label).toBe("test");
  });
});
