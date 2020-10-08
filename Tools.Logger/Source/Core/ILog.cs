using System;
using System.Runtime.CompilerServices;

namespace Tools.Logger
{
  public interface ILog
  {
    #region Public Methods

    void Debug(string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerMemberName] string methodName = null);

    void Error(string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerMemberName] string methodName = null);

    Exception Exception(string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerMemberName] string methodName = null);

    void Info(string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerMemberName] string methodName = null);

    void Other(string action,string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerMemberName] string methodName = null);

    void Warn(string key = null,object val = null,string note = null,object obj = null,string msg = null,Exception ex = null,[CallerMemberName] string methodName = null);

    #endregion Public Methods
  }
}
