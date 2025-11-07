namespace Cryptic.Core.Cryptography;

public record DerivedKey<TParameters>
{
    public required KeyDerivationFunctionName Function { get; init; }
    public required TParameters Parameters { get; init; }
    public required byte[] Key { get; init; }
}
