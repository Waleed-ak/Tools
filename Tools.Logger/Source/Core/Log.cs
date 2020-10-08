using System;
using System.Runtime.CompilerServices;

namespace Tools.Logger
{
  public class Log:ILog
  {
    #region Private Fields
    private readonly string ClassName;
    private readonly LogHandler Handler;
    #endregion Private Fields

    #region Public Constructors

    public Log(string className,LogHandler handler)
    {
      ClassName = className;
      Handler = handler;
    }

    #endregion Public Constructors

    #region Public Methods

    public void Debug(string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerMemberName] string methodName = null)
    => Handler.Debug(key,val,note,obj,msg,ex,ClassName,methodName);

    public void Error(string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerMemberName] string methodName = null)
    => Handler.Error(key,val,note,obj,msg,ex,ClassName,methodName);

    public Exception Exception(string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerMemberName] string methodName = null)
    => Handler.Exception(key,val,note,obj,msg,ex,ClassName,methodName);

    public void Info(string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerMemberName] string methodName = null)
    => Handler.Info(key,val,note,obj,msg,ex,ClassName,methodName);

    public void Other(string action,string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerMemberName] string methodName = null)
    => Handler.Other(action,key,val,note,obj,msg,ex,ClassName,methodName);

    public void Warn(string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerMemberName] string methodName = null)
    => Handler.Warn(key,val,note,obj,msg,ex,ClassName,methodName);

    #endregion Public Methods
  }
}
