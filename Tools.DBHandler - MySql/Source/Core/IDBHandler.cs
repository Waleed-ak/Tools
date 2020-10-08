using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tools.DBHandler
{
  public interface IDBHandler
  {
    #region Public Properties
    int CommandTimeOut { get; set; }
    string NameOrConnection { get; set; }
    #endregion Public Properties

    #region Public Methods

    int Add<T>(T item) where T : IEntity;

    void Add(Record record,Action<Record> action);

    IEnumerable<T> Add<T>(IEnumerable<T> items,int bulkCount = 10000) where T : IEntity;

    IEnumerable<Record> Add(IEnumerable<Record> records,Action<IEnumerable<Record>> action,int bulkCount = 10000);

    void AssignCastFunc<T>(Func<Record,T> func);

    int Delete<T>(T item) where T : IEntity;

    void Delete<T>(IEnumerable<T> items,int bulkCount = 10000) where T : IEntity;

    int Exec(string sqlQuery = "",Action<IParams> action = null);

    Task<int> ExecAsync(string sqlQuery = "",Action<IParams> action = null);

    DBSet<T> Set<T>() where T : IEntity;

    IEnumerable<Record> Sql(string sqlQuery = "",Action<IParams> action = null);

    IEnumerable<T> Sql<T>(string sqlQuery = "",Action<IParams> action = null,Func<Record,T> cast = null);

    IAsyncEnumerable<Record> SqlAsync(string sqlQuery = "",Action<IParams> action = null);

    Record SqlFirstOrDefault(string sqlQuery = "",Action<IParams> action = null);

    T SqlFirstOrDefault<T>(string sqlQuery = "",Action<IParams> action = null,Func<Record,T> cast = null) where T : class;

    IList<T> SqlList<T>(string sqlQuery,Action<IParams> action = null,Func<Record,T> cast = null);

    RecordSet SqlSet(string sqlQuery = "",Action<IParams> action = null);

    Task<RecordSet> SqlSetAsync(string sqlQuery = "",Action<IParams> action = null);

    List<RecordSet> SqlSets(string sqlQuery = "",Action<IParams> action = null);

    Task<List<RecordSet>> SqlSetsAsync(string sqlQuery = "",Action<IParams> action = null);

    int Update<T>(T item) where T : IEntity;

    void Update<T>(IEnumerable<T> items,int bulkCount = 10000) where T : IEntity;

    #endregion Public Methods
  }
}
