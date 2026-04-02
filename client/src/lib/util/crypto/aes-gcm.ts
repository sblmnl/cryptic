export interface AesGcmParameters {
  iv: Uint8Array | string;
  keyLen: 16 | 24 | 32;
}

export async function importAesGcmKey(key: Uint8Array) {
  return crypto.subtle.importKey("raw", new Uint8Array(key), "AES-GCM", true, ["encrypt", "decrypt"]);
}

export async function aesGcmEncrypt(data: Uint8Array, key: Uint8Array, iv: Uint8Array): Promise<Uint8Array> {
  const cryptoKey = await importAesGcmKey(key);
  const cipherText = await crypto.subtle.encrypt(
    { name: "AES-GCM", iv: new Uint8Array(iv) },
    cryptoKey,
    new Uint8Array(data),
  );
  return new Uint8Array(cipherText);
}

export async function aesGcmDecrypt(data: Uint8Array, key: Uint8Array, iv: Uint8Array): Promise<Uint8Array> {
  const cryptoKey = await importAesGcmKey(key);
  const plainText = await crypto.subtle.decrypt(
    { name: "AES-GCM", iv: new Uint8Array(iv) },
    cryptoKey,
    new Uint8Array(data),
  );
  return new Uint8Array(plainText);
}
