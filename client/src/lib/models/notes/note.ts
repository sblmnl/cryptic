import { aesGcmDecrypt, aesGcmEncrypt, type AesGcmParameters } from "@/lib/util/crypto/aes-gcm";
import { StandardArgon2Options, type Argon2PublicOptions } from "@/lib/util/crypto/argon2";
import type { SymmetricEncryptionMetadata } from "@/lib/util/crypto/metadata";
import { base64ToUint8, decodeUtf16le, encodeUtf16le, uint8ToBase64 } from "@/lib/util/encoding";
import sodium from "libsodium-wrappers-sumo";
import * as uuid from "uuid";

export type NoteId = Guid;

export interface NoteClientMetadata {
  clientName: string;
  clientVersion: string;
  encryptionOptions?: SymmetricEncryptionMetadata<AesGcmParameters, Argon2PublicOptions>;
}

export interface Note {
  id: NoteId;
  content: string;
  clientMetadata?: NoteClientMetadata;
}

export async function createNote(content: string, encryptionPassword?: string): Promise<Note> {
  if (!encryptionPassword || encryptionPassword.length === 0) {
    return {
      id: uuid.NIL,
      content,
    };
  }

  await sodium.ready;

  const hashOptions = StandardArgon2Options.owaspMostCpuIntensive();
  const derivedKey = sodium.crypto_pwhash(
    32,
    encryptionPassword,
    hashOptions.salt as Uint8Array,
    hashOptions.time,
    hashOptions.mem * 1024,
    sodium.crypto_pwhash_ALG_ARGON2ID13,
  );

  const encryptionOptions: SymmetricEncryptionMetadata<AesGcmParameters, Argon2PublicOptions> = {
    alg: "aes-gcm",
    params: {
      iv: crypto.getRandomValues(new Uint8Array(12)),
      keyLen: 32,
    },
    kdf: {
      func: "argon2",
      params: hashOptions,
    },
  };

  const plainText = encodeUtf16le(content);
  const cipherText = await aesGcmEncrypt(plainText, derivedKey, encryptionOptions.params.iv as Uint8Array);

  return {
    id: uuid.NIL,
    content: uint8ToBase64(cipherText),
    clientMetadata: {
      clientName: import.meta.env.VITE_CLIENT_NAME,
      clientVersion: import.meta.env.VITE_CLIENT_VERSION,
      encryptionOptions,
    },
  };
}

export async function decryptNote(note: Note, encryptionPassword: string): Promise<string> {
  await sodium.ready;

  const encryptionOptions = note.clientMetadata?.encryptionOptions as SymmetricEncryptionMetadata<
    AesGcmParameters,
    Argon2PublicOptions
  >;

  const salt = encryptionOptions.kdf!.params.salt;
  const saltBytes = salt instanceof Uint8Array ? salt : base64ToUint8(salt);

  const derivedKey = sodium.crypto_pwhash(
    encryptionOptions.params.keyLen,
    encryptionPassword,
    saltBytes,
    encryptionOptions.kdf!.params.time,
    encryptionOptions.kdf!.params.mem * 1024,
    sodium.crypto_pwhash_ALG_ARGON2ID13,
  );

  const cipherText = base64ToUint8(note.content);
  const iv = encryptionOptions.params.iv;
  const ivBytes = iv instanceof Uint8Array ? iv : base64ToUint8(iv);
  const plainText = await aesGcmDecrypt(cipherText, derivedKey, ivBytes);

  return decodeUtf16le(plainText);
}
