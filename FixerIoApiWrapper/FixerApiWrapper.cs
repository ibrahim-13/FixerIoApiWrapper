using System.Collections.Concurrent;
using System.Globalization;
using FixerIoApiWrapper.Models;
using FixerIoApiWrapper.Request;

namespace FixerIoApiWrapper;

public class FixerApiWrapper
{
    private readonly RequestClient _requestClient;

    private static void ValidateAccessKey(string accessKey)
    {
        if (string.IsNullOrWhiteSpace(accessKey) || string.IsNullOrEmpty(accessKey))
            throw new ArgumentNullException(nameof(accessKey));
    }

    public FixerApiWrapper(string accessKey) : this(accessKey, false) { }

    public FixerApiWrapper(string accessKey, HttpClient httpClient)
        : this(accessKey, httpClient, new ConcurrentDictionary<string, (string, HttpResponseMessage?)>()) { }

    public FixerApiWrapper(string accessKey, bool enableLogging)
        : this(accessKey, enableLogging ? new HttpClient() : new HttpClient(new LoggedHttpClientHandler(enableLogging))) { }

    public FixerApiWrapper(string accessKey, HttpClient httpClient, IDictionary<string, (string, HttpResponseMessage?)> cacheStorage)
    {
        ValidateAccessKey(accessKey);

        if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri(Constants.FixerIoBaseApi);

        _requestClient = new RequestClient(httpClient, cacheStorage);
        _requestClient.AddDefaultParameter(Constants.HeaderNameAccessKey, accessKey);
    }

    public async Task<SymbolResult?> GetSymbolsAsync(CancellationToken cancellationToken = default)
    {
        var builder = new UrlBuilder(Constants.FixerIoBaseApi);
        builder.SetPath(Constants.EndpointSymbols);
        return await _requestClient.GetCachedAsync<SymbolResult>(builder, cancellationToken);
    }

    public async Task<LatestRatesResult?> GetLatestRatesAsync(
        string? baseCurrencyCode = null,
        string[]? symbols = null,
        CancellationToken cancellationToken = default)
    {
        var builder = new UrlBuilder(Constants.FixerIoBaseApi);
        builder.SetPath(Constants.EndpointLatest);

        if (baseCurrencyCode != null)
            builder.AddParameter(Constants.ParameterBase, baseCurrencyCode);
        if (symbols is { Length: > 0 })
            builder.AddParameter(Constants.ParameterSymbols, string.Join(",", symbols));

        var response = await _requestClient.GetCachedAsync<LatestRatesResult>(builder, cancellationToken);

        return response;
    }

    public async Task<ConvertResult?> ConvertAsync(
        string fromCurrencyCode,
        string toCurrencyCode,
        decimal amount,
        DateTime date = default,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fromCurrencyCode) || string.IsNullOrEmpty(fromCurrencyCode))
            throw new ArgumentNullException(nameof(fromCurrencyCode));
        if (string.IsNullOrWhiteSpace(toCurrencyCode) || string.IsNullOrEmpty(toCurrencyCode))
            throw new ArgumentNullException(nameof(toCurrencyCode));
        if (amount == default)
            throw new ArgumentException($"Amount can not be {amount}", nameof(amount));
        
        var builder = new UrlBuilder(Constants.FixerIoBaseApi);
        builder.SetPath(Constants.EndpointConvert);

        builder.AddParameter(Constants.ParameterFrom, fromCurrencyCode);
        builder.AddParameter(Constants.ParameterTo, toCurrencyCode);
        builder.AddParameter(Constants.ParameterAmount, amount.ToString(CultureInfo.InvariantCulture));
        if (date != default)
            builder.AddParameter(Constants.ParameterDate, date.ToString(Constants.FormatDateForParameter));

        var response = await _requestClient.GetCachedAsync<ConvertResult>(builder, cancellationToken);

        return response;
    }
}
