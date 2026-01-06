import {
  aesCbcDecrypt,
  aesCbcEncrypt,
  StandardArgon2Options,
  type AesCbcParameters,
  type Argon2PublicOptions,
  type SymmetricEncryptionMetadata,
} from "@/core/crypto";
import * as argon2 from "argon2-browser/dist/argon2-bundled.min.js";

export type NoteId = Guid;

export interface NoteClientMetadata {
  clientName: string;
  clientVersion: string;
  encryptionOptions?: SymmetricEncryptionMetadata<object, object>;
}

export interface Note {
  content: string;
  clientMetadata?: NoteClientMetadata;
}

export async function createNote(content: string, encryptionPassword?: string): Promise<Note> {
  if (!encryptionPassword || encryptionPassword.length === 0) {
    return { content };
  }

  const hashOptions = StandardArgon2Options.owaspMostCpuIntensive();
  const derivedKey = await argon2.hash({ ...hashOptions, pass: encryptionPassword, hashLen: 32 });

  const encryptionOptions: SymmetricEncryptionMetadata<AesCbcParameters, Argon2PublicOptions> = {
    alg: "aes-cbc",
    params: {
      iv: crypto.getRandomValues(new Uint8Array(16)),
      keyLen: 32,
      paddingMode: "pkcs7",
    },
    kdf: {
      func: "argon2",
      params: hashOptions,
    },
  };

  const plainText = Buffer.from(content, "utf16le");
  const cipherText = await aesCbcEncrypt(plainText, derivedKey.hash, encryptionOptions.params.iv as Uint8Array);

  return {
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
    AesCbcParameters,
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
  const plainText = await aesCbcDecrypt(
    cipherText,
    derivedKey.hash,
    iv instanceof Uint8Array ? iv : Buffer.from(iv, "base64"),
  );

  return plainText.toString("utf16le");
}

export interface CreateNoteHttpRequest {
  content: string;
  deleteAfter: number;
  password?: string;
  clientMetadata?: string;
}

export interface CreateNoteHttpResponse {
  noteId: NoteId;
  controlToken: string;
}

export interface ReadNoteHttpResponse {
  noteId: NoteId;
  content: string;
  destroyed: boolean;
  clientMetadata: string | null;
}
