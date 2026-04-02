import { afterEach, describe, expect, it, vi } from "vitest";
import { base64ToUint8, uint8ToBase64 } from "@/lib/util/encoding";

// Mock libsodium to avoid slow Argon2 KDF in tests.
// Returns a deterministic 32-byte key derived from the password string.
const fakeKey = new Uint8Array(32);
vi.mock("libsodium-wrappers-sumo", () => {
  return {
    default: {
      ready: Promise.resolve(),
      crypto_pwhash: (keyLen: number, password: string) => {
        const key = new Uint8Array(keyLen);
        for (let i = 0; i < keyLen; i++) {
          key[i] = password.charCodeAt(i % password.length) ^ (i * 37);
        }
        fakeKey.set(key);
        return key;
      },
      crypto_pwhash_ALG_ARGON2ID13: 2,
    },
  };
});

import { createNote, decryptNote, type Note, type NoteClientMetadata } from "@/lib/models/notes/note";

afterEach(() => {
  vi.restoreAllMocks();
});

describe("createNote", () => {
  it("returns an unencrypted note when no password is provided", async () => {
    const note = await createNote("hello world");

    expect(note.content).toBe("hello world");
    expect(note.clientMetadata).toBeUndefined();
  });

  it("returns an unencrypted note when password is empty", async () => {
    const note = await createNote("hello world", "");

    expect(note.content).toBe("hello world");
    expect(note.clientMetadata).toBeUndefined();
  });

  it("returns an encrypted note when a password is provided", async () => {
    const note = await createNote("secret message", "mypassword");

    expect(note.content).not.toBe("secret message");
    expect(note.clientMetadata).toBeDefined();
    expect(note.clientMetadata!.encryptionOptions).toBeDefined();
    expect(note.clientMetadata!.encryptionOptions!.alg).toBe("aes-gcm");
    expect(note.clientMetadata!.encryptionOptions!.params.keyLen).toBe(32);
    expect(note.clientMetadata!.encryptionOptions!.kdf!.func).toBe("argon2");
  });

  it("produces base64-encoded ciphertext content", async () => {
    const note = await createNote("test", "password");

    // Content should be valid base64
    expect(() => base64ToUint8(note.content)).not.toThrow();
  });

  it("includes a 12-byte IV in encryption options", async () => {
    const note = await createNote("test", "password");
    const iv = note.clientMetadata!.encryptionOptions!.params.iv as Uint8Array;

    expect(iv).toBeInstanceOf(Uint8Array);
    expect(iv.byteLength).toBe(12);
  });

  it("includes Argon2 KDF parameters in encryption options", async () => {
    const note = await createNote("test", "password");
    const kdf = note.clientMetadata!.encryptionOptions!.kdf!;

    expect(kdf.params.type).toBe(2); // Argon2id
    expect(kdf.params.time).toBe(5);
    expect(kdf.params.mem).toBe(7_168);
    expect(kdf.params.parallelism).toBe(1);
    expect(kdf.params.salt).toBeInstanceOf(Uint8Array);
  });
});

describe("decryptNote", () => {
  it("decrypts an encrypted note back to the original content", async () => {
    const original = "Hello, Cryptic!";
    const note = await createNote(original, "password123");
    const decrypted = await decryptNote(note, "password123");

    expect(decrypted).toBe(original);
  });

  it("decrypts content containing unicode characters", async () => {
    const original = "Héllo wörld! \u{1F512}";
    const note = await createNote(original, "pass");
    const decrypted = await decryptNote(note, "pass");

    expect(decrypted).toBe(original);
  });

  it("fails to decrypt with the wrong password", async () => {
    const note = await createNote("secret", "correct-password");

    await expect(decryptNote(note, "wrong-password")).rejects.toThrow();
  });

  it("handles base64-encoded salt and IV from a deserialized note", async () => {
    const original = "round-trip test";
    const note = await createNote(original, "pass");

    // Simulate JSON serialization round-trip (as would happen via API),
    // which converts Uint8Arrays to base64 strings via the replacer.
    const metadata = note.clientMetadata!;
    const serialized: Note = {
      ...note,
      clientMetadata: {
        ...metadata,
        encryptionOptions: {
          ...metadata.encryptionOptions!,
          params: {
            ...metadata.encryptionOptions!.params,
            iv: uint8ToBase64(metadata.encryptionOptions!.params.iv as Uint8Array),
          },
          kdf: {
            ...metadata.encryptionOptions!.kdf!,
            params: {
              ...metadata.encryptionOptions!.kdf!.params,
              salt: uint8ToBase64(metadata.encryptionOptions!.kdf!.params.salt as Uint8Array),
            },
          },
        },
      } as NoteClientMetadata,
    };

    const decrypted = await decryptNote(serialized, "pass");
    expect(decrypted).toBe(original);
  });
});
