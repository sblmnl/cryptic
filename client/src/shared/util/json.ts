export function uint8ArrayToBase64Replacer(_key: string, value: any) {
  if (!(value instanceof Uint8Array)) return value;
  let str = "";
  for (const byte of value) str += String.fromCharCode(byte);
  return btoa(str);
}
