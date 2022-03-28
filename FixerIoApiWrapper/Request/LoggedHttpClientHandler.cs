namespace FixerIoApiWrapper.Request;

internal class LoggedHttpClientHandler : HttpClientHandler
{
    private readonly bool _isLoggingEnabled;

    public LoggedHttpClientHandler() : this(false) { }
    public LoggedHttpClientHandler(bool enableLogging) => _isLoggingEnabled = enableLogging;

    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!_isLoggingEnabled) return await base.SendAsync(request, cancellationToken);

        PrintRequestLog(request);
        var response = await base.SendAsync(request, cancellationToken);
        PrintResponseLog(response);
        return response;
    }

    private static async void PrintRequestLog(HttpRequestMessage request)
    {
        Console.WriteLine("Endpoint: {0}", request.RequestUri?.OriginalString);
        Console.WriteLine("Request Headers:");
        Console.WriteLine(request.ToString());
        Console.WriteLine("Request Body:");
        if (request.Content != null)
            Console.WriteLine(await request.Content.ReadAsStringAsync());
        Console.WriteLine();
    }

    private static async void PrintResponseLog(HttpResponseMessage response)
    {
        Console.WriteLine("Response Headers:");
        Console.WriteLine(response.ToString());
        Console.WriteLine("Response Body:");
        if (response.Content != null)
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        Console.WriteLine();
    }
}