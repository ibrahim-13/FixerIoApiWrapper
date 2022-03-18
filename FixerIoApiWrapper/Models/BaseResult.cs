using System.Text.Json.Serialization;

namespace FixerIoApiWrapper.Models;

public class BaseResultError
{
    [JsonPropertyName("code")]
    public int ErrorCode { get; set; }
    [JsonPropertyName("info")]
    public string Information { get; set; } = string.Empty;
}

public class BaseResult
{
    [JsonPropertyName("success")]
    public bool IsSuccess { get; set; }
    [JsonPropertyName("error")]
    public BaseResultError? Error { get; set; }
}
