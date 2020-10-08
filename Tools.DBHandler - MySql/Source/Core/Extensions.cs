using System;
using System.Linq;

namespace Tools.DBHandler
{
  public static class Extensions
  {
    #region Public Methods

    public static bool In<T>(this T item,params T[] items) => items.Contains(item);

    public static T With<T>(this T item,Action<T> action)
    {
      action(item);
      return item;
    }

    public static M With<T, M>(this T item,Func<T,M> func) => func(item);

    #endregion Public Methods
  }
}
