using System.IO;

namespace Tools.Logger
{
  public class LogFile:LogItem, ILogItem
  {
    #region Private Fields
    private readonly string Folder;
    #endregion Private Fields

    #region Public Constructors

    public LogFile(LogFileSettings setting)
    {
      ApplySettings(setting);
      Folder = setting.Folder;
    }

    #endregion Public Constructors

    #region Public Methods

    public void Log(LogValue value)
    {
      var path = Path.Combine(Folder,value.Application ?? "Default",$"{value.Time:yyyy-MM}",$"{value.Action}-{value.Time:yy-MM-dd}.log");
      var text = value.GetText();
      try
      {
        File.AppendAllText(path,text);
      }
      catch
      {
        var fileInfo = new FileInfo(path);
        Directory.CreateDirectory(fileInfo.DirectoryName);
        File.AppendAllText(path,text);
      }
    }

    #endregion Public Methods
  }
}
