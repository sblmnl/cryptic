import { aesGcmDecrypt, aesGcmEncrypt, type AesGcmParameters } from "@/lib/util/crypto/aes-gcm";
import { StandardArgon2Options, type Argon2PublicOptions } from "@/lib/util/crypto/argon2";
import type { SymmetricEncryptionMetadata } from "@/lib/util/crypto/metadata";
import * as argon2 from "argon2-browser/dist/argon2-bundled.min.js";
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

  const hashOptions = StandardArgon2Options.owaspMostCpuIntensive();
  const derivedKey = await argon2.hash({ ...hashOptions, pass: encryptionPassword, hashLen: 32 });

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

  const plainText = Buffer.from(content, "utf16le");
  const cipherText = await aesGcmEncrypt(plainText, derivedKey.hash, encryptionOptions.params.iv as Uint8Array);

  return {
    id: uuid.NIL,
    content: cipherText.toString("base64"),
    clientMetadata: {
      clientName: import.meta.env.VITE_CLIENT_NAME,
      clientVersion: import.meta.env.VITE_CLIENT_VERSION,
      encryptionOptions,
    },
  };
}

export async function decryptNote(note: Note, encryptionPassword: string): Promise<string> {
  const encryptionOptions = note.clientMetadata?.encryptionOptions as SymmetricEncryptionMetadata<
    AesGcmParameters,
    Argon2PublicOptions
  >;

  const salt = encryptionOptions.kdf!.params.salt;
  const derivedKey = await argon2.hash({
    ...encryptionOptions.kdf!.params,
    salt: salt instanceof Uint8Array ? salt : Buffer.from(salt, "base64"),
    pass: encryptionPassword,
    hashLen: encryptionOptions.params.keyLen,
  });

  const cipherText = Buffer.from(note.content, "base64");
  const iv = encryptionOptions.params.iv;
  const plainText = await aesGcmDecrypt(
    cipherText,
    derivedKey.hash,
    iv instanceof Uint8Array ? iv : Buffer.from(iv, "base64"),
  );

  return plainText.toString("utf16le");
}
