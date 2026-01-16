export interface Error {
  message: string;
  metadata: Record<string, object>;
  reasons: Error[];
}

export interface CodedError extends Error {
  code: string;
}
