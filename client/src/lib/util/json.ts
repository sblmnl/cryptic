export function uint8ArrayToBase64Replacer(_key: string, value: any) {
  return value instanceof Uint8Array ? Buffer.from(value).toString("base64") : value;
}
