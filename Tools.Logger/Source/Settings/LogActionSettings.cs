using System;
using System.Text.Json.Serialization;

namespace Tools.Logger
{
  public class LogActionSettings:LogItem
  {
    #region Public Properties

    [JsonIgnore]
    public Action<LogValue> Action { get; set; }

    #endregion Public Properties
  }
}
