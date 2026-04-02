using System.Text;

namespace Cryptic.Core.Tests.Helpers;

/// <summary>
/// Fast Argon2 parameters and hash factories for use in unit tests.
/// The production code uses OWASP-recommended (intentionally slow) parameters;
/// these use the absolute minimum to keep test suite runtime reasonable.
/// </summary>
internal static class TestFixtures
{
    public static Argon2Parameters FastArgon2Parameters => new()
    {
        Iterations = 1,
        MemoryAsKb = 8,
        Parallelism = 1,
        Salt = Rng.GetBytes(16),
    };

    public static ControlToken CreateControlToken() => ControlToken.Create();

    public static ControlTokenHash CreateControlTokenHash(ControlToken token)
    {
        var parameters = FastArgon2Parameters;
        var derivedKey = Argon2DerivedKey.Create(token.Value, parameters);
        return new ControlTokenHash
        {
            Function = KeyDerivationFunctionName.Argon2,
            Parameters = derivedKey.Parameters,
            Key = derivedKey.Key,
        };
    }

    public static PasswordHash CreatePasswordHash(string password)
    {
        var passwordBytes = Encoding.Unicode.GetBytes(password);
        var parameters = FastArgon2Parameters;
        var derivedKey = Argon2DerivedKey.Create(passwordBytes, parameters);
        return new PasswordHash
        {
            Function = KeyDerivationFunctionName.Argon2,
            Parameters = derivedKey.Parameters,
            Key = derivedKey.Key,
        };
    }

    public static Result<Note> CreateNote(
        string content = "Hello, world!",
        DeleteAfter deleteAfter = DeleteAfter.OneDay,
        ControlTokenHash? controlTokenHash = null,
        PasswordHash? passwordHash = null,
        string? clientMetadata = null)
    {
        var token = CreateControlToken();
        var hash = controlTokenHash ?? CreateControlTokenHash(token);
        return Note.Create(content, deleteAfter, hash, passwordHash, clientMetadata);
    }
}