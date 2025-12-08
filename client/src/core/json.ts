export function uint8ArrayToBase64Replacer(_key: string, value: any) {
  if (value instanceof Uint8Array) {
    return Buffer.from(value).toString("base64");
  }
  return value;
}
