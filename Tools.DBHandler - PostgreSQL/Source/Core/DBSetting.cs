using System;

namespace Tools.DBHandler
{
  public class DBSetting
  {
    #region Public Properties
    public int CommandTimeOut { set; get; } = 15;
    public Action<string> Log { get; set; }
    public string NameOrConnection { set; get; }
    #endregion Public Properties
  }
}
