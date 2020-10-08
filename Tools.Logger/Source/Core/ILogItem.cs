namespace Tools.Logger
{
  public interface ILogItem
  {
    #region Public Properties
    bool Active { get; set; }
    bool LogError { get; set; }
    bool LogInfo { get; set; }
    bool LogOther { get; set; }
    bool LogWarn { get; set; }
    #endregion Public Properties

    #region Public Methods

    void Log(LogValue value);

    #endregion Public Methods
  }
}
