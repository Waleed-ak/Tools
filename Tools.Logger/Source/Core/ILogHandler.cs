using System;
using System.Runtime.CompilerServices;

namespace Tools.Logger
{
  public interface ILogHandler
  {
    #region Public Methods

    void Debug(string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerFilePath] string filePath = null,[CallerMemberName]string methodName = null);

    void Error(string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerFilePath] string filePath = null,[CallerMemberName]string methodName = null);

    Exception Exception(string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerFilePath] string filePath = null,[CallerMemberName]string methodName = null);

    ILog GetLogger([CallerFilePath] string classname = null);

    void Info(string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerFilePath] string filePath = null,[CallerMemberName]string methodName = null);

    void Other(string other,string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerFilePath] string filePath = null,[CallerMemberName]string methodName = null);

    void Warn(string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerFilePath] string filePath = null,[CallerMemberName]string methodName = null);

    void Write(LogValue val);

    #endregion Public Methods
  }
}
