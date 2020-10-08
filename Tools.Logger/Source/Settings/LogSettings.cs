namespace Tools.Logger
{
  public class LogSettings
  {
    #region Public Properties
    public string Application { set; get; }
    public LogActionSettings LogActionSettings { get; set; } = new LogActionSettings();
    public LogConsoleSettings LogConsoleSettings { get; set; } = new LogConsoleSettings();
    public LogDebuggerSettings LogDebuggerSettings { get; set; } = new LogDebuggerSettings();
    public LogEmailSettings LogEmailSettings { get; set; } = new LogEmailSettings();
    public LogFileSettings LogFileSettings { get; set; } = new LogFileSettings();
    public LogSqlSettings LogSqlSettings { get; set; } = new LogSqlSettings();
    public LogWebApiSettings LogWebApiSettings { get; set; } = new LogWebApiSettings();
    public string Version { get; set; }
    #endregion Public Properties
  }
}
