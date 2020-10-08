namespace Tools.Logger
{
  public class LogSqlSettings:LogItem
  {
    #region Public Properties
    public string ConnectionStr { get; set; }

    public string TableName { get; set; } = "[Log].[tblLog]";
    #endregion Public Properties
  }
}
