using System;

namespace Tools.Logger
{
  public class LogAction:LogItem, ILogItem
  {
    #region Private Fields
    private readonly Action<LogValue> Action;
    #endregion Private Fields

    #region Public Constructors

    public LogAction(LogActionSettings setting)
    {
      ApplySettings(setting);
      Action = setting.Action;
    }

    #endregion Public Constructors

    #region Public Methods

    public void Log(LogValue value) => Action?.Invoke(value);

    #endregion Public Methods
  }
}
