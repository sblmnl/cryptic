import {
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
  clientMetdata?: NoteClientMetadata;
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
  const cipherText = await aesCbcEncrypt(plainText, derivedKey.hash, encryptionOptions.params.iv);

  return {
    content: cipherText.toString("base64"),
    clientMetdata: {
      clientName: import.meta.env.VITE_CLIENT_NAME,
      clientVersion: import.meta.env.VITE_CLIENT_VERSION,
      encryptionOptions,
    },
  };
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
