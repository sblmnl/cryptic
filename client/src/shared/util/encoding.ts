export function encodeUtf16le(str: string): Uint8Array {
  const buf = new ArrayBuffer(str.length * 2);
  const view = new DataView(buf);
  for (let i = 0; i < str.length; i++) {
    view.setUint16(i * 2, str.charCodeAt(i), true);
  }
  return new Uint8Array(buf);
}

export function decodeUtf16le(bytes: Uint8Array): string {
  const view = new DataView(bytes.buffer, bytes.byteOffset, bytes.byteLength);
  let str = "";
  for (let i = 0; i < bytes.byteLength; i += 2) {
    str += String.fromCharCode(view.getUint16(i, true));
  }
  return str;
}

export function uint8ToBase64(bytes: Uint8Array): string {
  let str = "";
  for (const byte of bytes) str += String.fromCharCode(byte);
  return btoa(str);
}

export function base64ToUint8(b64: string): Uint8Array {
  return Uint8Array.from(atob(b64), (c) => c.charCodeAt(0));
}
