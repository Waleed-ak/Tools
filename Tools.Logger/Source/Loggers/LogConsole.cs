using System;

namespace Tools.Logger
{
  public class LogConsole:LogItem, ILogItem
  {
    #region Public Constructors

    public LogConsole(LogConsoleSettings setting)
    {
      ApplySettings(setting);
    }

    #endregion Public Constructors

    #region Public Methods

    public void Log(LogValue value) => Console.WriteLine(value.GetText());

    #endregion Public Methods
  }
}
