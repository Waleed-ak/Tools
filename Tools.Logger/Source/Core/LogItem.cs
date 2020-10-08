namespace Tools.Logger
{
  public class LogItem
  {
    #region Public Properties
    public bool Active { get; set; }
    public bool LogError { get; set; }
    public bool LogInfo { get; set; }
    public bool LogOther { get; set; }
    public bool LogWarn { get; set; }
    #endregion Public Properties

    #region Internal Methods

    internal void ApplySettings(LogItem settings)
    {
      LogError = settings.LogError;
      LogInfo = settings.LogInfo;
      LogOther = settings.LogOther;
      LogWarn = settings.LogWarn;
    }

    #endregion Internal Methods
  }
}
