using System.Web;

namespace FixerIoApiWrapper.Request;

internal class UrlBuilder
{
    private readonly UriBuilder _uriBuilder;

    public UrlBuilder() => _uriBuilder = new UriBuilder();
    public UrlBuilder(string baseUrl) => _uriBuilder = new UriBuilder(baseUrl);

    public UrlBuilder SetPath(string path)
    {
        _uriBuilder.Path = path;
        return this;
    }

    public UrlBuilder AddParameter(string name, string value)
    {
        var hasPrev = _uriBuilder.Query == string.Empty;
        var param = HttpUtility.UrlEncode(name) + "=" + HttpUtility.UrlEncode(value);
        if (hasPrev)
            _uriBuilder.Query = _uriBuilder.Query + "&" + param;
        else
            _uriBuilder.Query = param;
        return this;
    }

    public Uri Uri() => _uriBuilder.Uri;

    public string Url() => _uriBuilder.Uri.OriginalString;
}