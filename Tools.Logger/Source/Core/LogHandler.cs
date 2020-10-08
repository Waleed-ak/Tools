using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Tools.Logger
{
  public class LogHandler:ILogHandler
  {
    #region Private Fields
    private static readonly IDictionary<string,Action<LogValue>> ActionItems = new Dictionary<string,Action<LogValue>>();
    private static bool IsLoaded;
    private readonly string Application;
    private readonly bool UseDebuger;
    private readonly string Version;
    #endregion Private Fields

    #region Public Constructors

    public LogHandler(Action<LogSettings> option,LogSettings initialSettings = null)
    {
      if(IsLoaded)
      {
        throw new Exception($"You cant Load this Model {nameof(LogHandler)} more than once.");
      }
      IsLoaded = true;
      var setting = initialSettings ?? new LogSettings();
      option?.Invoke(setting);
      SetupLogs(setting);
      Application = setting.Application;
      Version = setting.Version;
      UseDebuger = System.Diagnostics.Debugger.IsAttached;
    }

    #endregion Public Constructors

    #region Public Methods

    public void Debug(string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerFilePath] string filePath = null,[CallerMemberName]string methodName = null)
    {
      if(!UseDebuger)
        return;
      LocalLogger("Debug",ClassName(filePath),methodName,key,val,note,obj,msg,ex);
    }

    public void Error(string key,object val,string note,object obj,string msg,Exception ex,[CallerFilePath] string filePath = null,[CallerMemberName]string methodName = null)
      => LocalLogger("Error",ClassName(filePath),methodName,key,val,note,obj,msg,ex);

    public Exception Exception(string key,object val,string note,object obj,string msg,Exception ex,[CallerFilePath] string filePath = null,[CallerMemberName]string methodName = null)
    {
      Error(key,val,note,obj,msg,ex,filePath,methodName);
      return new Exception(msg,ex).Continue(c => c.Data["IsLogged"] = true);
    }

    public ILog GetLogger([CallerFilePath] string classname = null) => new Log(ClassName(classname),this);

    public void Info(string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerFilePath] string filePath = null,[CallerMemberName]string methodName = null)
      => LocalLogger("Info",ClassName(filePath),methodName,key,val,note,obj,msg,ex);

    public void Other(string other,string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerFilePath] string filePath = null,[CallerMemberName]string methodName = null)
      => LocalLogger(other,ClassName(filePath),methodName,key,val,note,obj,msg,ex);

    public void Warn(string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerFilePath] string filePath = null,[CallerMemberName]string methodName = null)
      => LocalLogger("Warn",ClassName(filePath),methodName,key,val,note,obj,msg,ex);

    public void Write(LogValue val)
      => Task.Run(() => GetAction(val.Action)?.Invoke(val));

    #endregion Public Methods

    #region Private Methods

    private string ClassName(string fileName) => fileName?.Split('\\').LastOrDefault();

    private Action<LogValue> GetAction(string str) => ActionItems.TryGetValue(str,out var val) ? val : ActionItems["Other"];

    private void LocalLogger(string action,string className,string methodName,string key,object val,string note,object obj,string msg,Exception ex)
      => Task.Run(() => GetAction(action)?.Invoke(new LogValue(Application,className,methodName,action,key,val,note,obj,msg ?? ex?.Message,ex,Version))).ConfigureAwait(false);

    private void SetupLogs(LogSettings setting)
    {
      var logItems = new List<ILogItem>();

      if(setting.LogActionSettings.Active)
      {
        logItems.Add(new LogAction(setting.LogActionSettings));
      }
      if(setting.LogConsoleSettings.Active)
      {
        logItems.Add(new LogConsole(setting.LogConsoleSettings));
      }
      if(setting.LogDebuggerSettings.Active)
      {
        logItems.Add(new LogDebugger(setting.LogDebuggerSettings));
      }
      if(setting.LogEmailSettings.Active)
      {
        logItems.Add(new LogEmail(setting.LogEmailSettings));
      }
      if(setting.LogFileSettings.Active)
      {
        logItems.Add(new LogFile(setting.LogFileSettings));
      }
      if(setting.LogSqlSettings.Active)
      {
        logItems.Add(new LogSql(setting.LogSqlSettings));
      }
      if(setting.LogWebApiSettings.Active)
      {
        logItems.Add(new LogWebApi(setting.LogWebApiSettings));
      }
      ActionItems["Error"] = null;
      ActionItems["Info"] = null;
      ActionItems["Other"] = null;
      ActionItems["Warn"] = null;
      ActionItems["Debug"] = null;
      foreach(var item in logItems)
      {
        if(UseDebuger)
          ActionItems["Debug"] += ItemLog(item);
        if(item.LogError)
          ActionItems["Error"] += ItemLog(item);
        if(item.LogInfo)
          ActionItems["Info"] += ItemLog(item);
        if(item.LogOther)
          ActionItems["Other"] += ItemLog(item);
        if(item.LogWarn)
          ActionItems["Warn"] += ItemLog(item);
      }
      Action<LogValue> ItemLog(ILogItem item) => c => Task.Run(() => item.Log(c)).ConfigureAwait(false);
    }

    #endregion Private Methods
  }
}
