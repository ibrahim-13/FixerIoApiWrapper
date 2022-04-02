using System.Net;
using System.Collections.Concurrent;
using System.Text.Json;
using FixerIoApiWrapper.Models;

namespace FixerIoApiWrapper.Request;

internal class RequestClient
{
    #region Variables
    private const string HeaderDateFormat = "ddd, dd MMM yyy HH:mm:ss GMT";
    private readonly IDictionary<string, (string, HttpResponseMessage?)> _cacheStorage;
    private readonly HttpClient _httpClient;

    private readonly List<(string name, string value)> _defaultParams = new();
    #endregion

    #region Constructors

    public RequestClient() : this(new HttpClient()) { }

    public RequestClient(HttpClient httpClient) : this(httpClient, new ConcurrentDictionary<string, (string, HttpResponseMessage?)>()) { }

    public RequestClient(HttpClient httpClient, IDictionary<string, (string, HttpResponseMessage?)> cacheStorage)
    {
        _httpClient = httpClient;
        _cacheStorage = cacheStorage;
    }
    #endregion

    #region Cache Keys
    private static string GetCacheKeyEtag(string uri) => $"{uri}__{Constants.HeaderNameEtag}";
    private static string GetCacheKeyDate(string uri) => $"{uri}__{Constants.HeaderNameDate}";
    private static string GetCacheKeyResponse(string uri) => uri;
    #endregion

    #region Cache Functions
    private void AddCacheControlHeaders(HttpRequestMessage request, Uri? uri)
    {
        if (uri is null) return;
        var originalString = uri.OriginalString;
        if (!_cacheStorage.TryGetValue(GetCacheKeyEtag(originalString), out (string, HttpResponseMessage?) val1) ||
            !_cacheStorage.TryGetValue(GetCacheKeyDate(originalString), out (string, HttpResponseMessage?) val2)) return;
        var (etag, _) = val1;
        var (date, _) = val2;
        request.Headers.Add(Constants.HeaderNameIfNoneMatch, etag);
        request.Headers.Add(Constants.HeaderNameIfModifiedSince, date);
    }

    private HttpResponseMessage? HandleEtagFromResponse(HttpResponseMessage? response, Uri? uri)
    {
        if (uri == null || response == null) return response;
        var originalString = uri.OriginalString;

        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            if (!_cacheStorage.TryGetValue(GetCacheKeyResponse(originalString),
                    out (string, HttpResponseMessage?) val1)) return response;
            var (_, cachedResponse) = val1;
            return cachedResponse;
        }

        var eTag = response.Headers.ETag?.Tag ?? string.Empty;
        var date = response.Headers.Date?.ToString(HeaderDateFormat) ?? string.Empty;

        if (eTag != string.Empty && date != string.Empty)
        {
            _cacheStorage[GetCacheKeyEtag(originalString)] = (eTag, null);
            _cacheStorage[GetCacheKeyDate(originalString)] = (date, null);
        }

        _cacheStorage[GetCacheKeyResponse(originalString)] = (string.Empty, response);
        return response;
    }
    #endregion

    public void AddDefaultHeader(string name, string value) => _httpClient.DefaultRequestHeaders.Add(name, value);

    public void AddDefaultQueryParameter(string name, string value) => _defaultParams.Add((name, value));

    public async Task<T?> GetCachedAsync<T>(UrlInfo urlBuilder, CancellationToken cancellationToken = default) where T : BaseResult
    {
        foreach (var (name, value) in _defaultParams)
            urlBuilder.AddQueryParameter(name, value);
        var uri = urlBuilder.Uri();
        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        AddCacheControlHeaders(request, uri);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var cachedResponse = HandleEtagFromResponse(response, uri);

        if (cachedResponse == null) return null;
        var content = await cachedResponse.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<T>(content);
    }
}