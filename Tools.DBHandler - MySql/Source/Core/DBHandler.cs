using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Tools.DBHandler
{
  internal interface IEnumrable<T>
  {
  }

  [SuppressMessage("Security","CA2100:Review SQL queries for security vulnerabilities")]
  public abstract class DBHandler<DB>:IDBHandler where DB : DBHandler<DB>
  {
    #region Private Fields
    private static readonly Dictionary<Type,Func<Record,object>> _Sql_Cast = new();
    private static readonly Dictionary<Type,Func<IEntity,int>> _Sql_Delete = new();
    private static readonly Dictionary<Type,Action<IEnumerable<IEntity>>> _Sql_Delete_Bulk = new();
    private static readonly Dictionary<Type,Func<IEntity,int>> _Sql_Insert = new();
    private static readonly Dictionary<Type,Action<IEnumerable<IEntity>>> _Sql_Insert_Bulk = new();
    private static readonly Dictionary<Type,Func<IEntity,int>> _Sql_Update = new();
    private static readonly Dictionary<Type,Action<IEnumerable<IEntity>>> _Sql_Update_Bulk = new();
    #endregion Private Fields

    #region Protected Constructors

    protected DBHandler(Action<DBSetting> option = null)
    {
      if(option is not null)
      {
        DBSetting setting = new();
        option(setting);
        NameOrConnection = setting.NameOrConnection;
        CommandTimeOut = setting.CommandTimeOut;
        Log = setting.Log;
      }
      ModelBuilder(new(this));
      AssignProperties();
    }

    #endregion Protected Constructors

    #region Public Properties
    public int CommandTimeOut { get; set; } = 15;
    public Action<string> Log { get; set; }
    public string NameOrConnection { get; set; }
    #endregion Public Properties

    #region Public Methods

    public int Add<T>(T item) where T : IEntity
    {
      if(_Sql_Insert.TryGetValue(typeof(T),out var res))
      {
        return res(item);
      }
      throw new($"Please Add Insert[Func] of type {typeof(T).Name} on the builder");
    }

    public void Add(Record record,Action<Record> action) => action(record);

    public IEnumerable<T> Add<T>(IEnumerable<T> items,int bulkCount = 10000) where T : IEntity
    {
      if(_Sql_Insert_Bulk.TryGetValue(typeof(T),out var res))
      {
        var count = 1;
        List<T> lst = new();
        foreach(var item in items)
        {
          lst.Add(item);
          if(++count > bulkCount)
          {
            res((IEnumerable<IEntity>)lst);
            lst.Clear();
            count = 1;
          }
          yield return item;
        }
        if(lst.Count > 0)
        {
          res((IEnumerable<IEntity>)lst);
          lst.Clear();
        }
      }
      throw new($"Please Add Insert Bulk[Func] of type {typeof(T).Name} on the builder");
    }

    public IEnumerable<Record> Add(IEnumerable<Record> records,Action<IEnumerable<Record>> action,int bulkCount = 10000)
    {
      var count = 1;
      List<Record> lst = new();
      foreach(var item in records)
      {
        lst.Add(item);
        if(++count > bulkCount)
        {
          action(lst);
          lst.Clear();
          count = 1;
        }
        yield return item;
      }
      if(lst.Count > 0)
      {
        action(lst);
        lst.Clear();
      }
    }

    public void AssignCastFunc<T>(Func<Record,T> func) => _Sql_Cast[typeof(T)] = new(x => func(x));

    public int Delete<T>(T item) where T : IEntity
    {
      if(_Sql_Delete.TryGetValue(typeof(T),out var res))
      {
        return res(item);
      }
      throw new($"Please Add Delete[Func] of type {typeof(T).Name} on the builder");
    }

    public void Delete<T>(IEnumerable<T> items,int bulkCount = 10000) where T : IEntity
    {
      if(_Sql_Delete_Bulk.TryGetValue(typeof(T),out var res))
      {
        var count = 1;
        List<T> lst = new();
        foreach(var item in items)
        {
          lst.Add(item);
          if(++count > bulkCount)
          {
            res((IEnumerable<IEntity>)lst);
            lst.Clear();
            count = 1;
          }
        }
        if(lst.Count > 0)
        {
          res((IEnumerable<IEntity>)lst);
          lst.Clear();
        }
      }
      throw new($"Please Add Delete Bulk[Action] of type {typeof(T).Name} on the builder");
    }

    public int Exec(string sqlQuery = "",Action<IParams> action = null)
    {
      using MySqlConnection connection = new(NameOrConnection);
      using var command = connection.CreateCommand();
      connection.Open();
      command.CommandTimeout = CommandTimeOut;
      command.CommandText = sqlQuery;
      Params.ApplyAction(command,action,Log);
      return command.ExecuteNonQuery();
    }

    public async Task<int> ExecAsync(string sqlQuery = "",Action<IParams> action = null)
    {
      using MySqlConnection connection = new(NameOrConnection);
      using var command = connection.CreateCommand();
      connection.Open();
      command.CommandTimeout = CommandTimeOut;
      command.CommandText = sqlQuery;
      await Params.ApplyActionAsync(command,action,Log).ConfigureAwait(false);
      return await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public DBSet<T> Set<T>() where T : IEntity => new(this);

    public IEnumerable<Record> Sql(string sqlQuery = "",Action<IParams> action = null)
    {
      using MySqlConnection connection = new(NameOrConnection);
      using var command = connection.CreateCommand();
      connection.Open();
      command.CommandTimeout = CommandTimeOut;
      command.CommandText = sqlQuery;
      Params.ApplyAction(command,action,Log);
      using(var reader = command.ExecuteReader())
      {
        var fc = reader.FieldCount;
        var names = new string[fc];
        for(var i = 0;i < fc;i++)
        {
          names[i] = reader.GetName(i);
        }
        while(reader.Read())
        {
          Record row = new();
          foreach(var name in names)
          {
            row[name] = reader[name];
          }
          yield return row;
        }
      }
      connection.Close();
    }

    public IEnumerable<T> Sql<T>(string sqlQuery = "",Action<IParams> action = null,Func<Record,T> cast = null)
    {
      cast ??= (_Sql_Cast.TryGetValue(typeof(T),out var res)) ? new Func<Record,T>(x => (T)res(x)) : x => x.CastObj<T>();
      using MySqlConnection connection = new(NameOrConnection);
      using var command = connection.CreateCommand();
      connection.Open();
      command.CommandTimeout = CommandTimeOut;
      command.CommandText = sqlQuery;
      Params.ApplyAction(command,action,Log);
      using(var reader = command.ExecuteReader())
      {
        var fc = reader.FieldCount;
        var names = new string[fc];
        for(var i = 0;i < fc;i++)
        {
          names[i] = reader.GetName(i);
        }
        while(reader.Read())
        {
          Record row = new();
          foreach(var name in names)
          {
            row[name] = reader[name];
          }
          yield return cast(row);
        }
      }
      connection.Close();
    }

    public async IAsyncEnumerable<Record> SqlAsync(string sqlQuery = "",Action<IParams> action = null)
    {
      using MySqlConnection connection = new(NameOrConnection);
      using var command = connection.CreateCommand();
      connection.Open();
      command.CommandTimeout = CommandTimeOut;
      command.CommandText = sqlQuery;
      Params.ApplyAction(command,action,Log);
      using(var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
      {
        var fc = reader.FieldCount;
        var names = new string[fc];
        for(var i = 0;i < fc;i++)
        {
          names[i] = reader.GetName(i);
        }
        while(await reader.ReadAsync().ConfigureAwait(false))
        {
          Record row = new();
          foreach(var name in names)
          {
            row[name] = reader[name];
          }
          yield return row;
        }
      }
      connection.Close();
    }

    public Record SqlFirstOrDefault(string sqlQuery = "",Action<IParams> action = null)
    {
      using MySqlConnection connection = new(NameOrConnection);
      using var command = connection.CreateCommand();
      connection.Open();
      command.CommandTimeout = CommandTimeOut;
      command.CommandText = sqlQuery;
      Params.ApplyAction(command,action,Log);
      using(var reader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow))
      {
        var fc = reader.FieldCount;
        var names = new string[fc];
        for(var i = 0;i < fc;i++)
        {
          names[i] = reader.GetName(i);
        }
        if(reader.Read())
        {
          Record row = new();
          foreach(var name in names)
          {
            row[name] = reader[name];
          }
          return row;
        }
        else
        {
          return null;
        }
      }
    }

    public T SqlFirstOrDefault<T>(string sqlQuery = "",Action<IParams> action = null,Func<Record,T> cast = null) where T : class
    {
      cast ??= (_Sql_Cast.TryGetValue(typeof(T),out var res)) ? new Func<Record,T>(x => (T)res(x)) : x => x.CastObj<T>();
      using MySqlConnection connection = new(NameOrConnection);
      using var command = connection.CreateCommand();
      connection.Open();
      command.CommandTimeout = CommandTimeOut;
      command.CommandText = sqlQuery;
      Params.ApplyAction(command,action,Log);
      using(var reader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow))
      {
        var fc = reader.FieldCount;
        var names = new string[fc];
        for(var i = 0;i < fc;i++)
        {
          names[i] = reader.GetName(i);
        }
        if(reader.Read())
        {
          Record row = new();
          foreach(var name in names)
          {
            row[name] = reader[name];
          }
          return cast(row);
        }
        else
        {
          return null;
        }
      }
    }

    public IList<T> SqlList<T>(string sqlQuery,Action<IParams> action = null,Func<Record,T> cast = null)
    {
      List<T> result = new();
      cast ??= _Sql_Cast.TryGetValue(typeof(T),out var res) ? new Func<Record,T>(x => (T)res(x)) : x => x.CastObj<T>();
      using MySqlConnection connection = new(NameOrConnection);
      using var command = connection.CreateCommand();
      connection.Open();
      command.CommandTimeout = CommandTimeOut;
      command.CommandText = sqlQuery;
      Params.ApplyAction(command,action,Log);
      using(var reader = command.ExecuteReader())
      {
        var fc = reader.FieldCount;
        var names = new string[fc];
        for(var i = 0;i < fc;i++)
        {
          names[i] = reader.GetName(i);
        }
        while(reader.Read())
        {
          Record row = new();
          foreach(var name in names)
          {
            row[name] = reader[name];
          }
          result.Add(cast(row));
        }
      }
      connection.Close();
      return result;
    }

    public RecordSet SqlSet(string sqlQuery = "",Action<IParams> action = null)
    {
      RecordSet result = new();
      using MySqlConnection connection = new(NameOrConnection);
      using var command = connection.CreateCommand();
      connection.Open();
      command.CommandTimeout = CommandTimeOut;
      command.CommandText = sqlQuery;
      Params.ApplyAction(command,action,Log);
      using(var reader = command.ExecuteReader())
      {
        var fc = reader.FieldCount;
        var names = new string[fc];
        for(var i = 0;i < fc;i++)
        {
          names[i] = reader.GetName(i);
        }
        while(reader.Read())
        {
          Record row = new();
          foreach(var name in names)
          {
            row[name] = reader[name];
          }
          result.Add(row);
        }
      }
      connection.Close();
      return result;
    }

    public async Task<RecordSet> SqlSetAsync(string sqlQuery = "",Action<IParams> action = null)
    {
      RecordSet result = new();
      using MySqlConnection connection = new(NameOrConnection);
      using var command = connection.CreateCommand();
      connection.Open();
      command.CommandTimeout = CommandTimeOut;
      command.CommandText = sqlQuery;
      await Params.ApplyActionAsync(command,action,Log).ConfigureAwait(false);
      using(var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
      {
        var fc = reader.FieldCount;
        var names = new string[fc];
        for(var i = 0;i < fc;i++)
        {
          names[i] = reader.GetName(i);
        }
        while(await reader.ReadAsync().ConfigureAwait(false))
        {
          Record row = new();
          foreach(var name in names)
          {
            row[name] = reader[name];
          }
          result.Add(row);
        }
      }
      connection.Close();
      return result;
    }

    public List<RecordSet> SqlSets(string sqlQuery = "",Action<IParams> action = null)
    {
      List<RecordSet> _Sets = new();
      using(MySqlConnection connection = new(NameOrConnection))
      using(var command = connection.CreateCommand())
      {
        connection.Open();
        command.CommandTimeout = CommandTimeOut;
        command.CommandText = sqlQuery;
        Params.ApplyAction(command,action,Log);
        using var reader = command.ExecuteReader();
        do
        {
          RecordSet _Set = new();
          var fc = reader.FieldCount;
          var names = new string[fc];
          for(var i = 0;i < fc;i++)
          {
            names[i] = reader.GetName(i);
          }
          while(reader.Read())
          {
            Record row = new();
            foreach(var name in names)
            {
              row[name] = reader[name];
            }
            _Set.Add(row);
          }
          _Sets.Add(_Set);
        } while(reader.NextResult());
      }
      return _Sets;
    }

    public async Task<List<RecordSet>> SqlSetsAsync(string sqlQuery = "",Action<IParams> action = null)
    {
      List<RecordSet> _Sets = new();
      using(MySqlConnection connection = new(NameOrConnection))
      using(var command = connection.CreateCommand())
      {
        connection.Open();
        command.CommandTimeout = CommandTimeOut;
        command.CommandText = sqlQuery;
        Params.ApplyAction(command,action,Log);
        using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
        do
        {
          RecordSet _Set = new();
          var fc = reader.FieldCount;
          var names = new string[fc];
          for(var i = 0;i < fc;i++)
          {
            names[i] = reader.GetName(i);
          }
          while(await reader.ReadAsync().ConfigureAwait(false))
          {
            Record row = new();
            foreach(var name in names)
            {
              row[name] = reader[name];
            }
            _Set.Add(row);
          }
          _Sets.Add(_Set);
        } while(await reader.NextResultAsync().ConfigureAwait(false));
      }
      return _Sets;
    }

    public int Update<T>(T item) where T : IEntity
    {
      if(_Sql_Update.TryGetValue(typeof(T),out var res))
      {
        return res(item);
      }
      throw new($"Please Add Update[Func] of type {typeof(T).Name} on the builder");
    }

    public void Update<T>(IEnumerable<T> items,int bulkCount = 10000) where T : IEntity
    {
      if(_Sql_Update_Bulk.TryGetValue(typeof(T),out var res))
      {
        var count = 1;
        List<T> lst = new();
        foreach(var item in items)
        {
          lst.Add(item);
          if(++count > bulkCount)
          {
            res((IEnumerable<IEntity>)items);
            lst.Clear();
            count = 1;
          }
        }
        if(lst.Count > 0)
        {
          res((IEnumerable<IEntity>)items);
          lst.Clear();
        }
      }
      throw new($"Please Add Update Bulk[Action] of type {typeof(T).Name} on the builder");
    }

    #endregion Public Methods

    #region Internal Methods

    internal void AssignDelete<T>(Func<T,int> func) where T : IEntity => _Sql_Delete[typeof(T)] = new(x => func((T)x));

    internal void AssignDeleteBulk<T>(Action<IEnumerable<T>> action) where T : IEntity => _Sql_Delete_Bulk[typeof(T)] = new(x => action((IEnumerable<T>)x));

    internal void AssignInsert<T>(Func<T,int> func) where T : IEntity => _Sql_Insert[typeof(T)] = new(x => func((T)x));

    internal void AssignInsertBulk<T>(Action<IEnumerable<T>> action) where T : IEntity => _Sql_Insert_Bulk[typeof(T)] = new(x => action((IEnumerable<T>)x));

    internal void AssignUpdate<T>(Func<T,int> func) where T : IEntity => _Sql_Update[typeof(T)] = new(x => func((T)x));

    internal void AssignUpdateBulk<T>(Action<IEnumerable<T>> action) where T : IEntity => _Sql_Update_Bulk[typeof(T)] = new(x => action((IEnumerable<T>)x));

    #endregion Internal Methods

    #region Protected Methods

    protected abstract void ModelBuilder(Builder<DB> builder);

    #endregion Protected Methods

    #region Private Methods

    private void AssignProperties()
    {
      var tt = typeof(DBSet<>);
      var properties = GetType()
        .GetProperties(bindingAttr: System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
        .Where(c => c.PropertyType.IsGenericType && c.PropertyType.GetGenericTypeDefinition() == tt);
      foreach(var item in properties)
      {
        SetProperty(tt,item);
      }

      void SetProperty(Type tt,System.Reflection.PropertyInfo item)
      {
        var value = item.GetValue(this);
        if(value is null)
        {
          var typeArgs = item.PropertyType.GetGenericArguments();
          var makeme = tt.MakeGenericType(typeArgs);
          var obj = Activator.CreateInstance(makeme,args: new[] { this });
          item.SetValue(this,obj);
        }
      }
    }

    #endregion Private Methods
  }
}
