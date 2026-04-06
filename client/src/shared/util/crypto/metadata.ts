export type KdfName = "argon2";

export interface KdfMetadata<TParams> {
  func: KdfName;
  params: TParams;
}

export type SymmetricEncryptionAlgorithm = "aes-gcm";

export interface SymmetricEncryptionMetadata<TAlgParams, TKdfParams> {
  alg: SymmetricEncryptionAlgorithm;
  params: TAlgParams;
  kdf?: KdfMetadata<TKdfParams>;
}
