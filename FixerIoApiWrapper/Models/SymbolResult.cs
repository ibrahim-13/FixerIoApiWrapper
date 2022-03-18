using System.Text.Json.Serialization;

namespace FixerIoApiWrapper.Models;

public class SymbolResult : BaseResult
{
    [JsonPropertyName("symbols")]
    public Dictionary<string, string>? Symbols { get; set; }
}
