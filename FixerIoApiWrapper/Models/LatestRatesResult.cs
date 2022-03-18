using System.Text.Json.Serialization;

namespace FixerIoApiWrapper.Models;

public class LatestRatesResult : BaseResult
{
    [JsonPropertyName("timestamp")]
    public int Timestamp { get; set; }
    [JsonPropertyName("base")]
    public string BaseCurrencyCode { get; set; } = string.Empty;
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
    [JsonPropertyName("rates")]
    public Dictionary<string, decimal>? ConversionRates { get; set; }
}
