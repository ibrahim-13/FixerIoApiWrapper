using System.Collections.Concurrent;
using System.Globalization;
using FixerIoApiWrapper.Models;
using FixerIoApiWrapper.Request;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace FixerIoApiWrapper;

public class FixerIoApiWrapperOptions
{
    /// <summary>
    /// Base Url for API requests
    /// </summary>
    public string? BaseUrl { get; set; }
    /// <summary>
    /// Use the provided HttpClient for making API calls
    /// </summary>
    public HttpClient? HttpClient { get; set; }
    /// <summary>
    /// Cache Storage for API responses
    /// </summary>
    public IDictionary<string, (string, HttpResponseMessage?)>? CacheStorage { get; set; }
    /// <summary>
    /// Log API responses for debugging.
    /// NOTE: This options will not work if HttpClient is provided in the options.
    /// </summary>
    public bool? EnableApiResponseLogging { get; set; }
}

public class FixerApiWrapper
{
    private readonly RequestClient _requestClient;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="accessKey">Access Key provided by Fixer.io</param>
    /// <param name="opt">Options</param>
    /// <exception cref="ArgumentNullException">Providing null, empty or whitespace value for access key will throw exception</exception>
    public FixerApiWrapper(string accessKey, FixerIoApiWrapperOptions? opt = default)
    {
        if (string.IsNullOrWhiteSpace(accessKey) || string.IsNullOrEmpty(accessKey))
            throw new ArgumentNullException(nameof(accessKey));

        var httpClient = opt?.HttpClient ?? (opt?.EnableApiResponseLogging == true
            ? new HttpClient(new LoggedHttpClientHandler(true))
            : new HttpClient());

        _requestClient = new RequestClient(
            httpClient,
            opt?.CacheStorage ?? new ConcurrentDictionary<string, (string, HttpResponseMessage?)>());
        _requestClient.AddDefaultParameter(Constants.HeaderNameAccessKey, accessKey);
    }

    public async Task<SymbolResult?> GetSymbolsAsync(CancellationToken cancellationToken = default)
    {
        var builder = GetUrlInfoWithPath(Constants.EndpointSymbols);
        return await _requestClient.GetCachedAsync<SymbolResult>(builder, cancellationToken);
    }

    public async Task<LatestRatesResult?> GetLatestRatesAsync(
        string? baseCurrencyCode = null,
        string[]? symbols = null,
        CancellationToken cancellationToken = default)
    {
        var builder = GetUrlInfoWithPath(Constants.EndpointLatest);

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

        var builder = GetUrlInfoWithPath(Constants.EndpointConvert);

        builder.AddParameter(Constants.ParameterFrom, fromCurrencyCode);
        builder.AddParameter(Constants.ParameterTo, toCurrencyCode);
        builder.AddParameter(Constants.ParameterAmount, amount.ToString(CultureInfo.InvariantCulture));
        if (date != default)
            builder.AddParameter(Constants.ParameterDate, date.ToString(Constants.FormatDateForParameter));

        var response = await _requestClient.GetCachedAsync<ConvertResult>(builder, cancellationToken);

        return response;
    }

    private static UrlInfo GetUrlInfoWithPath(string path) => new UrlInfo(Constants.FixerIoBaseApi)
        .SetPath(path);
}
