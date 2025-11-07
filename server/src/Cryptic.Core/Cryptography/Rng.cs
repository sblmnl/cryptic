using System.Security.Cryptography;

namespace Cryptic.Core.Cryptography;

public static class Rng
{
    public static byte[] GetBytes(int count)
    {
        byte[] bytes = new byte[count];
        RandomNumberGenerator.Fill(bytes);
        return bytes;
    }
}
