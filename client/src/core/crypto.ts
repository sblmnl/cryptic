import { ArgonType, type Argon2BrowserHashOptions } from "argon2-browser/dist/argon2-bundled.min.js";

export interface Argon2DifficultyOptions {
  time: number;
  mem: number;
  parallelism: number;
}

export interface Argon2PublicOptions extends Argon2DifficultyOptions {
  type: ArgonType;
  salt: Uint8Array | string;
}

export function createArgon2PublicOptions(
  time: number,
  mem: number,
  parallelism: number,
  saltLen = 16,
  type = ArgonType.Argon2id,
): Argon2PublicOptions {
  return {
    type,
    time,
    mem,
    parallelism,
    salt: saltLen > 0 ? crypto.getRandomValues(new Uint8Array(saltLen)) : new Uint8Array(),
  };
}

export abstract class StandardArgon2Options {
  static owaspMostMemoryIntensive = (saltLen = 16) => createArgon2PublicOptions(1, 47_104, 1, saltLen);
  static owaspSomewhatBalanced = (saltLen = 16) => createArgon2PublicOptions(2, 19_456, 1, saltLen);
  static owaspBalanced = (saltLen = 16) => createArgon2PublicOptions(3, 12_288, 1, saltLen);
  static owaspSomewhatCpuIntensive = (saltLen = 16) => createArgon2PublicOptions(4, 9_216, 1, saltLen);
  static owaspMostCpuIntensive = (saltLen = 16) => createArgon2PublicOptions(5, 7_168, 1, saltLen);
}

export function createArgon2Options(
  pass: string,
  difficulty: Argon2DifficultyOptions,
  saltLength = 16,
): Argon2BrowserHashOptions {
  return {
    pass,
    salt: crypto.getRandomValues(new Uint8Array(saltLength)),
    ...difficulty,
  };
}

export type KdfName = "argon2";

export interface KdfMetadata<TParams> {
  func: KdfName;
  params: TParams;
}

export type PaddingMode = "pkcs7";

export type SymmetricEncryptionAlgorithm = "aes-cbc";

export interface AesCbcParameters {
  iv: Uint8Array | string;
  keyLen: 16 | 32;
  paddingMode: PaddingMode;
}

export interface SymmetricEncryptionMetadata<TAlgParams, TKdfParams> {
  alg: SymmetricEncryptionAlgorithm;
  params: TAlgParams;
  kdf?: KdfMetadata<TKdfParams>;
}

export function importAesCbcKey(key: Uint8Array) {
  return crypto.subtle.importKey(
    "raw",
    Buffer.from(key),
    {
      name: "aes-cbc",
      length: key.length * 8,
    },
    true,
    ["encrypt", "decrypt"],
  );
}

export async function aesCbcEncrypt(data: Uint8Array, key: Uint8Array, iv: Uint8Array) {
  const cryptoKey = await importAesCbcKey(key);
  const dataBuf = Buffer.from(data);
  const ivBuf = Buffer.from(iv);

  const cipherText = await crypto.subtle.encrypt({ name: "aes-cbc", iv: ivBuf }, cryptoKey, dataBuf);

  return Buffer.from(cipherText);
}

export async function aesCbcDecrypt(data: Uint8Array, key: Uint8Array, iv: Uint8Array) {
  const cryptoKey = await importAesCbcKey(key);
  const dataBuf = Buffer.from(data);
  const ivBuf = Buffer.from(iv);

  const plainText = await crypto.subtle.decrypt({ name: "aes-cbc", iv: ivBuf }, cryptoKey, dataBuf);

  return Buffer.from(plainText);
}
