import { describe, expect, it } from "vitest";
import { base64ToUint8, decodeUtf16le, encodeUtf16le, uint8ToBase64 } from "@/lib/util/encoding";

describe("encodeUtf16le", () => {
  it("encodes an empty string", () => {
    expect(encodeUtf16le("")).toEqual(new Uint8Array([]));
  });

  it("encodes ASCII characters", () => {
    const result = encodeUtf16le("AB");
    expect(result).toEqual(new Uint8Array([0x41, 0x00, 0x42, 0x00]));
  });

  it("encodes non-ASCII characters", () => {
    const result = encodeUtf16le("\u00e9"); // é
    expect(result).toEqual(new Uint8Array([0xe9, 0x00]));
  });
});

describe("decodeUtf16le", () => {
  it("decodes an empty buffer", () => {
    expect(decodeUtf16le(new Uint8Array([]))).toBe("");
  });

  it("decodes ASCII characters", () => {
    expect(decodeUtf16le(new Uint8Array([0x41, 0x00, 0x42, 0x00]))).toBe("AB");
  });

  it("round-trips with encodeUtf16le", () => {
    const input = "Hello, world! \u00e9\u00f1\u00fc";
    expect(decodeUtf16le(encodeUtf16le(input))).toBe(input);
  });
});

describe("uint8ToBase64", () => {
  it("encodes an empty array", () => {
    expect(uint8ToBase64(new Uint8Array([]))).toBe("");
  });

  it("encodes bytes to base64", () => {
    // "Hello" in ASCII
    const bytes = new Uint8Array([72, 101, 108, 108, 111]);
    expect(uint8ToBase64(bytes)).toBe("SGVsbG8=");
  });
});

describe("base64ToUint8", () => {
  it("decodes an empty string", () => {
    expect(base64ToUint8("")).toEqual(new Uint8Array([]));
  });

  it("decodes base64 to bytes", () => {
    expect(base64ToUint8("SGVsbG8=")).toEqual(new Uint8Array([72, 101, 108, 108, 111]));
  });

  it("round-trips with uint8ToBase64", () => {
    const bytes = new Uint8Array([0, 1, 127, 128, 255]);
    expect(base64ToUint8(uint8ToBase64(bytes))).toEqual(bytes);
  });
});
