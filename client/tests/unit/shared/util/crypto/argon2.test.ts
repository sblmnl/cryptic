import { ArgonType, StandardArgon2Options, createArgon2PublicOptions } from "@/shared/util/crypto/argon2";
import { describe, expect, it } from "vitest";

describe("ArgonType", () => {
  it("defines the correct enum values", () => {
    expect(ArgonType.Argon2d).toBe(0);
    expect(ArgonType.Argon2i).toBe(1);
    expect(ArgonType.Argon2id).toBe(2);
  });
});

describe("createArgon2PublicOptions", () => {
  it("creates options with the given parameters", () => {
    const opts = createArgon2PublicOptions(3, 12288, 1, 16, ArgonType.Argon2id);
    expect(opts.time).toBe(3);
    expect(opts.mem).toBe(12288);
    expect(opts.parallelism).toBe(1);
    expect(opts.type).toBe(ArgonType.Argon2id);
    expect(opts.salt).toBeInstanceOf(Uint8Array);
    expect((opts.salt as Uint8Array).byteLength).toBe(16);
  });

  it("defaults to Argon2id with 16-byte salt", () => {
    const opts = createArgon2PublicOptions(1, 1024, 2);
    expect(opts.type).toBe(ArgonType.Argon2id);
    expect((opts.salt as Uint8Array).byteLength).toBe(16);
  });

  it("creates an empty salt when saltLen is 0", () => {
    const opts = createArgon2PublicOptions(1, 1024, 1, 0);
    expect((opts.salt as Uint8Array).byteLength).toBe(0);
  });
});

describe("StandardArgon2Options", () => {
  it("owaspMostMemoryIntensive uses time=1, mem=47104", () => {
    const opts = StandardArgon2Options.owaspMostMemoryIntensive();
    expect(opts.time).toBe(1);
    expect(opts.mem).toBe(47_104);
    expect(opts.parallelism).toBe(1);
  });

  it("owaspMostCpuIntensive uses time=5, mem=7168", () => {
    const opts = StandardArgon2Options.owaspMostCpuIntensive();
    expect(opts.time).toBe(5);
    expect(opts.mem).toBe(7_168);
    expect(opts.parallelism).toBe(1);
  });

  it("generates unique salts on each call", () => {
    const a = StandardArgon2Options.owaspBalanced();
    const b = StandardArgon2Options.owaspBalanced();
    expect(a.salt).not.toEqual(b.salt);
  });

  it("respects custom salt length", () => {
    const opts = StandardArgon2Options.owaspBalanced(32);
    expect((opts.salt as Uint8Array).byteLength).toBe(32);
  });
});
