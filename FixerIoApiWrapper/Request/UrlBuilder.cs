using System.Web;

namespace FixerIoApiWrapper.Request;

internal class UrlBuilder
{
    private readonly UriBuilder _uriBuilder;

    public UrlBuilder(string baseUrl) => _uriBuilder = new UriBuilder(baseUrl);

    public void SetPath(string path) => _uriBuilder.Path = path;

    public void AddParameter(string name, string value)
    {
        var hasPrev = _uriBuilder.Query == string.Empty;
        var param = HttpUtility.UrlEncode(name) + "=" + HttpUtility.UrlEncode(value);
        if (hasPrev)
            _uriBuilder.Query = _uriBuilder.Query + "&" + param;
        else
            _uriBuilder.Query = param;
    }

    public Uri Uri() => _uriBuilder.Uri;

    public string Url() => _uriBuilder.Uri.OriginalString;
}