using System;
using System.Runtime.Serialization;

namespace Tools.Logger
{
  [Serializable]
  public sealed class LoggedException:Exception
  {
    #region Public Constructors

    public LoggedException()
    {
      Data["ILogged"] = true;
      IsLogged = true;
    }

    public LoggedException(Exception innerException) : base(innerException.Message,innerException)
    {
      Data["ILogged"] = true;
      IsLogged = true;
    }

    public LoggedException(string message) : base(message)
    {
      Data["ILogged"] = true;
      IsLogged = true;
    }

    public LoggedException(string message,Exception innerException) : base(message,innerException)
    {
      Data["ILogged"] = true;
      IsLogged = true;
    }

    private LoggedException(SerializationInfo info,StreamingContext context) : base(info,context)
    {
      Data["ILogged"] = true;
      IsLogged = true;
    }

    #endregion Public Constructors

    #region Public Properties
    public bool IsLogged { get; set; }
    #endregion Public Properties
  }
}
