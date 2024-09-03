using System.Text.Json.Serialization;

namespace Cryptic.WebAPI.Common.Http;

public class ApiResponseBody
{
    public static readonly ApiResponseBody InternalError = new ApiResponseBody
    {
        Data = null,
        Errors = [ HttpErrors.InternalError ]
    };
    
    public string Status => Errors.Any() ? "error" : "ok";
    public object? Data { get; init; } = null;
    public IEnumerable<CodedError> Errors { get; init; } = [];
    public IEnumerable<string> Warnings { get; init; } = [];
    
    [JsonPropertyName("_meta")]
    public MetaInformation Meta { get; init; } = new();
}
