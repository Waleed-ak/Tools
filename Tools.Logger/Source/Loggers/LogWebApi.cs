using System.Net.Http;
using System.Net.Http.Formatting;

namespace Tools.Logger
{
  public class LogWebApi:LogItem, ILogItem
  {
    #region Private Fields
    private readonly string Uri;
    #endregion Private Fields

    #region Public Constructors

    public LogWebApi(LogWebApiSettings setting)
    {
      ApplySettings(setting);
      Uri = setting.Uri;
    }

    #endregion Public Constructors

    #region Public Methods

    public void Log(LogValue value)
    {
      using var client = new HttpClient();
      client.PostAsync(Uri,value,new JsonMediaTypeFormatter()).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    #endregion Public Methods
  }
}
