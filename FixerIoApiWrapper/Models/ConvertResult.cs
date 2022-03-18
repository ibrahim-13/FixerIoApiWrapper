using System.Text.Json.Serialization;

namespace FixerIoApiWrapper.Models;

public class ConvertResultQuery
{
    [JsonPropertyName("from")]
    public string FromCurrencyCode { get; set; } = string.Empty;

    [JsonPropertyName("to")]
    public string ToCurrencyCode { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public int Amount { get; set; } = 0;
}

public class ConvertResultInfo
{
    [JsonPropertyName("timestamp")]
    public int Timestamp { get; set; }
    [JsonPropertyName("rate")]
    public decimal ConversionRate { get; set; }
}

public class ConvertResult : BaseResult
{
    [JsonPropertyName("query")]
    public ConvertResultQuery? Query { get; set; }
    [JsonPropertyName("info")]
    public ConvertResultInfo? ResultInfo { get; set; }
    [JsonPropertyName("historical")]
    public bool IsHistorical { get; set; }
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
    [JsonPropertyName("result")]
    public decimal Result { get; set; }
}
