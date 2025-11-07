// ReSharper disable InconsistentNaming

namespace Cryptic.Core.Cryptography;

public record Argon2Parameters
{
    public const int DefaultSaltLength = 32;

    public Argon2Type Type { get; init; } = Argon2Type.Argon2id;
    public Argon2Version Version { get; init; } =  Argon2Version.Version13;
    public int Iterations { get; init; } = 3;
    public int MemoryAsKb { get; init; } = 12_288;
    public int Parallelism { get; init; } = 1;
    public byte[] Salt { get; init; } = Rng.GetBytes(DefaultSaltLength);
    public byte[] Secret { get; init; } = [];
    public byte[] Additional { get; init; } = [];

    public Org.BouncyCastle.Crypto.Parameters.Argon2Parameters ToBouncyCastleArgon2Parameters() =>
        new Org.BouncyCastle.Crypto.Parameters.Argon2Parameters.Builder((int)Type)
            .WithVersion((int)Version)
            .WithIterations(Iterations)
            .WithMemoryAsKB(MemoryAsKb)
            .WithParallelism(Parallelism)
            .WithSalt(Salt)
            .WithSecret(Secret)
            .WithAdditional(Additional)
            .Build();

    public static Argon2Parameters FromBouncyCastleArgon2Parameters(
        Org.BouncyCastle.Crypto.Parameters.Argon2Parameters parameters)
    {
        return new()
        {
            Type = (Argon2Type)parameters.Type,
            Version = (Argon2Version)parameters.Version,
            Iterations = parameters.Iterations,
            MemoryAsKb = parameters.Memory,
            Parallelism = parameters.Parallelism,
            Salt = parameters.GetSalt(),
            Additional = parameters.GetAdditional(),
            Secret = parameters.GetSecret(),
        };
    }

    public static Argon2Parameters OwaspMostMemoryIntensive(int saltLength = DefaultSaltLength) => new()
    {
        Salt = Rng.GetBytes(saltLength),
        Iterations = 1,
        MemoryAsKb = 47_104,
        Parallelism = 1,
    };

    public static Argon2Parameters OwaspSomewhatBalanced(int saltLength = DefaultSaltLength) => new()
    {
        Salt = Rng.GetBytes(saltLength),
        Iterations = 2,
        MemoryAsKb = 19_456,
        Parallelism = 1,
    };

    public static Argon2Parameters OwaspBalanced(int saltLength = DefaultSaltLength) => new()
    {
        Salt = Rng.GetBytes(saltLength),
        Iterations = 3,
        MemoryAsKb = 12_288,
        Parallelism = 1,
    };

    public static Argon2Parameters OwaspSomewhatCpuIntensive(int saltLength = DefaultSaltLength) => new()
    {
        Salt = Rng.GetBytes(saltLength),
        Iterations = 4,
        MemoryAsKb = 9_216,
        Parallelism = 1,
    };

    public static Argon2Parameters OwaspMostCpuIntensive(int saltLength = DefaultSaltLength) => new()
    {
        Salt = Rng.GetBytes(saltLength),
        Iterations = 5,
        MemoryAsKb = 7_168,
        Parallelism = 1,
    };

    public static implicit operator Org.BouncyCastle.Crypto.Parameters.Argon2Parameters(Argon2Parameters parameters) =>
        parameters.ToBouncyCastleArgon2Parameters();

    public static implicit operator Argon2Parameters(Org.BouncyCastle.Crypto.Parameters.Argon2Parameters parameters) =>
        FromBouncyCastleArgon2Parameters(parameters);
}
