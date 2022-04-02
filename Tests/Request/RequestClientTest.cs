using Xunit;
using System;
using System.Linq;
using System.Net.Http;
using FixerIoApiWrapper.Request;
using System.Threading.Tasks;
using System.Threading;
using FixerIoApiWrapper.Models;
using System.Web;
using System.Collections.Generic;
using FixerIoApiWrapper;

namespace Tests.Request;

internal class StubHttpMessageHandler : DelegatingHandler
{
    private static readonly HttpResponseMessage _default = new() {
        StatusCode = System.Net.HttpStatusCode.OK,
        Content = new StringContent("{}"),
    };

    public Action<HttpRequestMessage>? OnBeforeSendAsync { get; set; }
    public HttpResponseMessage? ResponseMessage { get; set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        OnBeforeSendAsync?.Invoke(request);
        return Task.FromResult(ResponseMessage ?? _default);
    }
}

public class RequestClientTest : IDisposable
{
    private readonly StubHttpMessageHandler _stubHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly RequestClient _requestClient;
    public RequestClientTest()
    {
        _stubHttpMessageHandler = new StubHttpMessageHandler();
        _httpClient = new HttpClient(_stubHttpMessageHandler);
        _requestClient = new RequestClient(_httpClient);
    }

    [Fact]
    public async Task Can_Set_DefaultHeaders()
    {
        const string headerName = "test-header";
        const string headerValue = "test-header-value";
        _requestClient.AddDefaultHeader(headerName, headerValue);
        _stubHttpMessageHandler.OnBeforeSendAsync = delegate (HttpRequestMessage request) {
            if (request.Headers.TryGetValues(headerName, out var values)) {
                if (!values.Contains(headerValue)) throw new Exception("Header value not found");
            }
            else throw new Exception("Header not found");
        };
        var urlInfo = new UrlInfo("https://base.com");
        var exception = await Record.ExceptionAsync(() => _requestClient.GetCachedAsync<BaseResult>(urlInfo));
        Assert.Null(exception);
    }

    [Fact]
    public async Task Can_Set_DefaultParameters()
    {
        const string paramName = "test-param";
        const string paramValue = "test-param-value";
        _requestClient.AddDefaultQueryParameter(paramName, paramValue);
        _stubHttpMessageHandler.OnBeforeSendAsync = delegate (HttpRequestMessage request) {
            if (!(HttpUtility.ParseQueryString(request.RequestUri?.Query ?? string.Empty).Get(paramName) == paramValue))
                throw new Exception("Query parameter not found");
        };
        var urlInfo = new UrlInfo("https://base.com");
        var exception = await Record.ExceptionAsync(() => _requestClient.GetCachedAsync<BaseResult>(urlInfo));
        Assert.Null(exception);
    }

    [Fact]
    public async Task Should_UseCaching_ForRequestsWithEtag()
    {
        var dict = new Dictionary<string, (string, HttpResponseMessage?)>();
        _stubHttpMessageHandler.ResponseMessage = new HttpResponseMessage {
            Content = new StringContent("{}"),
            StatusCode = System.Net.HttpStatusCode.OK,
        };
        _stubHttpMessageHandler.ResponseMessage.Headers.ETag = new System.Net.Http.Headers.EntityTagHeaderValue("\"etag\"");
        _stubHttpMessageHandler.ResponseMessage.Headers.Add("Date", DateTimeOffset.UtcNow.ToString(Constants.HeaderDateFormat));
        var requestClient = new RequestClient(_httpClient, dict);
        var urlInfo = new UrlInfo("https://base.com");
        await requestClient.GetCachedAsync<BaseResult>(urlInfo);
        Assert.Equal(3, dict.Count);

        _stubHttpMessageHandler.ResponseMessage = new HttpResponseMessage {
            // Using emtpy string, if the cache is used then it will not throw JsonException,
            // as it will not try to deserialize empty string
            Content = new StringContent(""),
            StatusCode = System.Net.HttpStatusCode.NotModified,
        };
        _stubHttpMessageHandler.OnBeforeSendAsync = delegate (HttpRequestMessage request) {
            if (request.Headers.TryGetValues(Constants.HeaderNameIfNoneMatch, out var _) &&
            request.Headers.TryGetValues(Constants.HeaderNameIfModifiedSince, out var _)
            ) { }
            else throw new Exception("Headers value not found");
        };
        _stubHttpMessageHandler.ResponseMessage.Headers.ETag = new System.Net.Http.Headers.EntityTagHeaderValue("\"etag\"");
        _stubHttpMessageHandler.ResponseMessage.Headers.Add("Date", DateTimeOffset.UtcNow.ToString(Constants.HeaderDateFormat));
        var exception = await Record.ExceptionAsync(() => requestClient.GetCachedAsync<BaseResult>(urlInfo));
        Assert.Null(exception);
    }

    public void Dispose()
    {
        _stubHttpMessageHandler.OnBeforeSendAsync = null;
        _stubHttpMessageHandler.ResponseMessage = null;
    }
}
