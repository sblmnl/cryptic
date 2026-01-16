export interface AesGcmParameters {
  iv: Uint8Array | string;
  keyLen: 16 | 24 | 32;
}

export async function importAesGcmKey(key: Uint8Array) {
  return crypto.subtle.importKey("raw", Buffer.from(key), "AES-GCM", true, ["encrypt", "decrypt"]);
}

export async function aesGcmEncrypt(data: Uint8Array, key: Uint8Array, iv: Uint8Array) {
  const cryptoKey = await importAesGcmKey(key);
  const dataBuf = Buffer.from(data);
  const ivBuf = Buffer.from(iv);

  const cipherText = await crypto.subtle.encrypt({ name: "AES-GCM", iv: ivBuf }, cryptoKey, dataBuf);

  return Buffer.from(cipherText);
}

export async function aesGcmDecrypt(data: Uint8Array, key: Uint8Array, iv: Uint8Array) {
  const cryptoKey = await importAesGcmKey(key);
  const dataBuf = Buffer.from(data);
  const ivBuf = Buffer.from(iv);

  const plainText = await crypto.subtle.decrypt({ name: "AES-GCM", iv: ivBuf }, cryptoKey, dataBuf);

  return Buffer.from(plainText);
}
