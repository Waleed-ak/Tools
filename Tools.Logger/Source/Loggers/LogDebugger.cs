using System.Diagnostics;

namespace Tools.Logger
{
  public class LogDebugger:LogItem, ILogItem
  {
    #region Public Constructors

    public LogDebugger(LogDebuggerSettings setting)
    {
      ApplySettings(setting);
    }

    #endregion Public Constructors

    #region Public Methods

    public void Log(LogValue value) => Debug.Write(value.GetText());

    #endregion Public Methods
  }
}
