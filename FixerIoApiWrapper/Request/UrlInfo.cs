using System.Web;

namespace FixerIoApiWrapper.Request;

internal class UrlInfo
{
    private readonly UriBuilder _uriBuilder;

    public UrlInfo(string baseUrl) => _uriBuilder = new UriBuilder(baseUrl);

    public UrlInfo SetPath(string path)
    {
        _uriBuilder.Path = path;
        return this;
    }

    public UrlInfo AddParameter(string name, string value)
    {
        var hasPrev = _uriBuilder.Query != string.Empty;
        var param = HttpUtility.UrlEncode(name) + "=" + HttpUtility.UrlEncode(value);
        if (hasPrev)
            _uriBuilder.Query = _uriBuilder.Query + "&" + param;
        else
            _uriBuilder.Query = param;
        return this;
    }

    public Uri Uri() => _uriBuilder.Uri;

    public string Url() => _uriBuilder.Uri.AbsoluteUri;
}