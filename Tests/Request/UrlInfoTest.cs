using Xunit;
using FixerIoApiWrapper.Request;
using System.Web;

namespace Tests.Request;
public class UrlInfoTest {
    [Fact]
    public void Can_Set_BaseUrl()
    {
        var baseUrl = "https://baseurl.com/";
        var urlInfo = new UrlInfo(baseUrl);
        Assert.Equal(baseUrl, urlInfo.Url());
        Assert.Equal(baseUrl, urlInfo.Uri().AbsoluteUri);
    }

    [Fact]
    public void Can_Set_Path()
    {
        var baseUrl = "https://baseurl.com";
        var path = "/somepath";
        var fullUrl = baseUrl + path;

        var urlInfo = new UrlInfo(baseUrl);
        urlInfo.SetPath(path);
        Assert.Equal(fullUrl, urlInfo.Url());
        Assert.Equal(fullUrl, urlInfo.Uri().AbsoluteUri);
        Assert.Equal(path, urlInfo.Uri().PathAndQuery);
    }

    [Fact]
    public void Can_Add_Parameter()
    {
        string actual;
        var url = "https://baseurl.com/path";

        // Single Parameter
        var urlInfo = new UrlInfo(url)
            .AddQueryParameter("param1", "value1");
        actual = url + "?param1=value1";
        Assert.Equal(actual, urlInfo.Url());
        Assert.Equal(actual, urlInfo.Uri().AbsoluteUri);

        // Multiple Parameter
        var urlInfo2 = new UrlInfo(url)
            .AddQueryParameter("param1", "value1")
            .AddQueryParameter("param2", "value2");
        actual = url + "?param1=value1&param2=value2";
        Assert.Equal(actual, urlInfo2.Url());
        Assert.Equal(actual, urlInfo2.Uri().AbsoluteUri);

        // Parameter that needs to be URL Encoded
        var urlInfo3 = new UrlInfo(url)
            .AddQueryParameter("param 1", "value 1");
        actual = $"{url}?{HttpUtility.UrlEncode("param 1")}={HttpUtility.UrlEncode("value 1")}";
        Assert.Equal(actual, urlInfo3.Url());
        Assert.Equal(actual, urlInfo3.Uri().AbsoluteUri);
    }
}
