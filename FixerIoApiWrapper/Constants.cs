using System.Net;

namespace FixerIoApiWrapper;

internal static class Constants
{
    #region Fixer.io Endpoints info
    public const string FixerIoBaseApi = "https://data.fixer.io/api/";
    public const string EndpointSymbols = "symbols";
    public const string EndpointLatest = "latest";
    public const string EndpointConvert = "convert";
    #endregion

    #region Header names
    public const string HeaderNameEtag = nameof(HttpResponseHeader.ETag);
    public const string HeaderNameDate = nameof(HttpResponseHeader.Date);
    public const string HeaderNameAccessKey = "access_key";
    public const string HeaderNameIfNoneMatch = "If-None-Match";
    public const string HeaderNameIfModifiedSince = "If-Modified-Since";
    #endregion

    #region Parameter names
    public const string ParameterBase = "base";
    public const string ParameterSymbols = "symbols";
    public const string ParameterFrom = "from";
    public const string ParameterTo = "to";
    public const string ParameterAmount = "amount";
    public const string ParameterDate = "date";
    #endregion

    #region Others
    public const string FormatDateForParameter = "yyyy-MM-dd";
    #endregion
}
