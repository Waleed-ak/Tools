using System;
using System.Collections.Generic;

namespace Tools.DBHandler
{
  public class DBSet<T> where T : IEntity
  {
    #region Private Fields
    private readonly IDBHandler Handler;
    #endregion Private Fields

    #region Public Constructors

    public DBSet(IDBHandler dBHandler)
    {
      Handler = dBHandler;
    }

    #endregion Public Constructors

    #region Public Methods

    public void Add(T item) => Handler.Add(item);

    public void Add(IEnumerable<T> items,int bulkCount = 10000) => Handler.Add(items,bulkCount);

    public void Delete(T item) => Handler.Delete(item);

    public void Delete(IEnumerable<T> items,int bulkCount = 10000) => Handler.Delete(items,bulkCount);

    public IEnumerable<T> Sql(string sqlQuery,Action<IParams> action = null) => Handler.Sql<T>(sqlQuery,action);

    public IList<T> SqlList(string sqlQuery,Action<IParams> action = null) => Handler.SqlList<T>(sqlQuery,action);

    public void Update(T item) => Handler.Update(item);

    public void Update(IEnumerable<T> items,int bulkCount = 10000) => Handler.Update(items,bulkCount);

    #endregion Public Methods
  }
}
