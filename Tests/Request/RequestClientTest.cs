using Xunit;
using Moq;
using Moq.Protected;
using System.Net.Http;
using FixerIoApiWrapper.Request;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;

namespace Tests.Request;

internal class MockHttpMessageHandler : DelegatingHandler
{
    private static readonly HttpResponseMessage _default = new(System.Net.HttpStatusCode.OK);
    private HttpResponseMessage? _responseMessage { get; set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_responseMessage ?? _default);
    }
}

public class RequestClientTest
{
    [Fact]
    public void Can_Set_Global_Headers()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();

        var httpClient = new HttpClient(mockHttpMessageHandler);
        var requestClient = new RequestClient(httpClient);
    }
}
