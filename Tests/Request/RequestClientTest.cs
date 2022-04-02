using Xunit;
using System;
using System.Linq;
using System.Net.Http;
using FixerIoApiWrapper.Request;
using System.Threading.Tasks;
using System.Threading;
using FixerIoApiWrapper.Models;

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

public class RequestClientTest
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
    public async Task Can_Set_Global_Headers()
    {
        _requestClient.AddDefaultHeader("test-header", "test-header-value");
        _mockHttpMessageHandler.OnBeforeSendAsync = delegate (HttpRequestMessage request) {
            if (request.Headers.TryGetValues("test-header", out var values)) {
                if (!values.Contains("test-header-value")) throw new Exception("Header value not found");
            }
            else throw new Exception("Header not found");
        };
        var urlInfo = new UrlInfo("https://base.com");
        //await _requestClient.GetCachedAsync<BaseResult>(urlInfo);
        var exception = await Record.ExceptionAsync(() => _requestClient.GetCachedAsync<BaseResult>(urlInfo));
        Assert.Null(exception);
        _mockHttpMessageHandler.OnBeforeSendAsync = null;
    }
}
