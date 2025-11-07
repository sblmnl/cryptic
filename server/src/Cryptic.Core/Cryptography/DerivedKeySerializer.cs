using Newtonsoft.Json;
using System.Text;

namespace Cryptic.Core.Cryptography;

public static class DerivedKeySerializer
{
    public static string Serialize<TParameters>(DerivedKey<TParameters> derivedKey)
    {
        string jsonString = JsonConvert.SerializeObject(derivedKey);
        byte[] jsonStringBytes = Encoding.UTF8.GetBytes(jsonString);
        return Convert.ToBase64String(jsonStringBytes);
    }

    public static DerivedKey<TParameters> Deserialize<TParameters>(string derivedKeyString)
    {
        byte[] jsonBytes = Convert.FromBase64String(derivedKeyString);
        string jsonString = Encoding.UTF8.GetString(jsonBytes);

        var derivedKey = JsonConvert.DeserializeObject<DerivedKey<TParameters>>(jsonString)
            ?? throw new FormatException("Invalid derived key!");

        return derivedKey;
    }
}
