using System;
using System.Text;

namespace Tools.Logger
{
  public static class Extensions
  {
    #region Public Methods

    public static string GetDescription(this Exception e)
      => new StringBuilder().AddException(e).ToString();

    public static string GetFriendlyName(this Type type)
    {
      if(type is null)
        return null;

      var friendlyName = type.Name;
      if(friendlyName.Contains("AnonymousType"))
        return "AnonymousType";
      if(type.IsGenericType)
      {
        var iBacktick = friendlyName.IndexOf('`');
        if(iBacktick > 0)
        {
          friendlyName = friendlyName.Remove(iBacktick);
        }
        friendlyName += "<";
        var typeParameters = type.GetGenericArguments();
        for(var i = 0;i < typeParameters.Length;++i)
        {
          var typeParamName = typeParameters[i].IsGenericType ? GetFriendlyName(typeParameters[i]) : typeParameters[i].Name;
          friendlyName += (i == 0 ? "" : ",") + typeParamName;
        }
        friendlyName += ">";
      }
      return friendlyName;
    }

    #endregion Public Methods

    #region Internal Methods

    internal static T Continue<T>(this T @this,Action<T> action)
    {
      action(@this);
      return @this;
    }

    #endregion Internal Methods

    #region Private Methods

    private static StringBuilder AddException(this StringBuilder builder,Exception e,int seq = 0)
    {
      builder.Append("Exception Type: ")
        .Append(e.GetType().GetFriendlyName())
        .Append(" Source: ")
        .AppendLine(e.Source)
        .Append("Message: ")
        .AppendLine(e.Message)
        .Append("Stack Trace: ")
        .AppendLine(e.StackTrace);
      e.InnerException?.Continue(c => builder.Append("Inner Exception Depth: ").Append(seq).AppendLine().AddException(c,++seq));
      return builder;
    }

    #endregion Private Methods
  }
}
