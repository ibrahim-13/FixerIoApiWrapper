namespace FixerIoApiWrapper.Request;

internal class LoggedHttpClientHandler : HttpClientHandler
{
    private readonly bool _isLoggingEnabled;

    public LoggedHttpClientHandler() : this(false) { }
    public LoggedHttpClientHandler(bool enableLogging) => _isLoggingEnabled = enableLogging;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!_isLoggingEnabled) return await base.SendAsync(request, cancellationToken);

        // Do logging here
        var response = await base.SendAsync(request, cancellationToken);
        return response;
    }
}