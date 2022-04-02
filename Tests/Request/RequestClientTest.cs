using Xunit;
using System;
using System.Linq;
using System.Net.Http;
using FixerIoApiWrapper.Request;
using System.Threading.Tasks;
using System.Threading;
using FixerIoApiWrapper.Models;
using System.Web;

namespace Tests.Request;

internal class MockHttpMessageHandler : DelegatingHandler
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
    private readonly MockHttpMessageHandler _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly RequestClient _requestClient;
    public RequestClientTest()
    {
        _mockHttpMessageHandler = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_mockHttpMessageHandler);
        _requestClient = new RequestClient(_httpClient);
    }

    [Fact]
    public async Task Can_Set_DefaultHeaders()
    {
        const string headerName = "test-header";
        const string headerValue = "test-header-value";
        _requestClient.AddDefaultHeader(headerName, headerValue);
        _mockHttpMessageHandler.OnBeforeSendAsync = delegate (HttpRequestMessage request) {
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
        _mockHttpMessageHandler.OnBeforeSendAsync = delegate (HttpRequestMessage request) {
            if (!(HttpUtility.ParseQueryString(request.RequestUri?.Query ?? string.Empty).Get(paramName) == paramValue))
                throw new Exception("Query parameter not found");
        };
        var urlInfo = new UrlInfo("https://base.com");
        var exception = await Record.ExceptionAsync(() => _requestClient.GetCachedAsync<BaseResult>(urlInfo));
        Assert.Null(exception);
    }

    public void Dispose()
    {
        _mockHttpMessageHandler.OnBeforeSendAsync = null;
        _mockHttpMessageHandler.ResponseMessage = null;
    }
}
