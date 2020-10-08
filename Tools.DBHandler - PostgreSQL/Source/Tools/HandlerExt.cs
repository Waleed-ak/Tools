using System;
using System.Collections.Generic;

namespace Tools.DBHandler
{
  public static class HandlerExt
  {
    #region Public Methods

    public static IEnumerable<T> DBAdd<T>(this IEnumerable<T> items,IDBHandler handler,int bulkCount = 10000) where T : IEntity
      => handler.Add(items,bulkCount);

    public static IEnumerable<Record> DBAdd(this IEnumerable<Record> items,IDBHandler handler,Action<IEnumerable<Record>> action,int bulkCount = 10000)
      => handler.Add(items,action,bulkCount);

    #endregion Public Methods
  }
}
