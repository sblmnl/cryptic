import { aesGcmDecrypt, aesGcmEncrypt, importAesGcmKey } from "@/shared/util/crypto/aes-gcm";
import { describe, expect, it } from "vitest";

describe("importAesGcmKey", () => {
  it("imports a 256-bit key", async () => {
    const key = crypto.getRandomValues(new Uint8Array(32));
    const cryptoKey = await importAesGcmKey(key);
    expect(cryptoKey.algorithm).toMatchObject({ name: "AES-GCM" });
    expect(cryptoKey.usages).toContain("encrypt");
    expect(cryptoKey.usages).toContain("decrypt");
  });
});

describe("aesGcmEncrypt / aesGcmDecrypt", () => {
  it("round-trips plaintext through encrypt and decrypt", async () => {
    const key = crypto.getRandomValues(new Uint8Array(32));
    const iv = crypto.getRandomValues(new Uint8Array(12));
    const plaintext = new TextEncoder().encode("Hello, Cryptic!");

    const ciphertext = await aesGcmEncrypt(plaintext, key, iv);
    expect(ciphertext).not.toEqual(plaintext);
    expect(ciphertext.byteLength).toBeGreaterThan(plaintext.byteLength);

    const decrypted = await aesGcmDecrypt(ciphertext, key, iv);
    expect(decrypted).toEqual(plaintext);
  });

  it("fails to decrypt with a wrong key", async () => {
    const key = crypto.getRandomValues(new Uint8Array(32));
    const wrongKey = crypto.getRandomValues(new Uint8Array(32));
    const iv = crypto.getRandomValues(new Uint8Array(12));
    const plaintext = new TextEncoder().encode("secret");

    const ciphertext = await aesGcmEncrypt(plaintext, key, iv);

    await expect(aesGcmDecrypt(ciphertext, wrongKey, iv)).rejects.toThrow();
  });

  it("fails to decrypt with a wrong IV", async () => {
    const key = crypto.getRandomValues(new Uint8Array(32));
    const iv = crypto.getRandomValues(new Uint8Array(12));
    const wrongIv = crypto.getRandomValues(new Uint8Array(12));
    const plaintext = new TextEncoder().encode("secret");

    const ciphertext = await aesGcmEncrypt(plaintext, key, iv);

    await expect(aesGcmDecrypt(ciphertext, key, wrongIv)).rejects.toThrow();
  });
});
