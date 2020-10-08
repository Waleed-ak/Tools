using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tools.DBHandler
{
  public class Params:IParams
  {
    #region Private Fields
    private readonly SqlCommand Command;
    private readonly List<SqlParameter> Parameters = new();
    private int CommandTimeOut = 0;
    private Action<string> Log;
    private int Sequence = 1;
    #endregion Private Fields

    #region Public Constructors

    public Params(SqlCommand command)
    {
      Command = command;
    }

    #endregion Public Constructors

    #region Public Methods

    public static void ApplyAction(SqlCommand command,Action<IParams> action,Action<string> log)
    {
      if(action is null)
        return;
      command.Parameters.Clear();
      Params temp = new(command);
      action(temp);
      if(temp.Log is null)
      {
        temp.Log = log;
      }
      temp.LogParameters();
      temp.UpdateCommand();

      command.Parameters.AddRange(temp.Parameters.ToArray());
    }

    public static Task ApplyActionAsync(SqlCommand command,Action<IParams> action,Action<string> log) => Task.Run(() => ApplyAction(command,action,log));

    public IParams Add(object value,string param = null)
    {
      Parameters.Add(FromValue(value,param));
      return this;
    }

    public IParams AddJson(object value,string param = null)
    {
      if(value == null)
      {
        Add(null,param);
      }
      else
      {
        Parameters.Add(ObjectToJson(value,param));
      }
      return this;
    }

    public IParams AddSqlParam(SqlParameter value,string param = null)
    {
      value.ParameterName = FixName(param);
      Parameters.Add(value);
      return this;
    }

    public IParams AddValues(params object[] values)
    {
      foreach(var item in values)
      {
        Add(item);
      }
      return this;
    }

    public IParams SetLog(Action<string> log)
    {
      Log = log;
      return this;
    }

    public IParams SetTimeOut(int value)
    {
      CommandTimeOut = value;
      return this;
    }

    #endregion Public Methods

    #region Protected Methods

    protected void LogParameters()
    {
      if(Log is null)
        return;
      StringBuilder str = new();
      foreach(var item in Parameters)
      {
        str
            .Append(item.DbType)
            .Append("  ")
            .Append(item.ParameterName)
            .Append("  ")
            .Append(item.Value)
            .AppendLine();
      }
      str.AppendLine(Command.CommandText);
      Log(str.ToString());
    }

    #endregion Protected Methods

    #region Private Methods

    private string FixName(string name)
    {
      if(string.IsNullOrEmpty(name))
      {
        return $"@{Sequence++}";
      }
      return name.StartsWith("@") ? name : $"@{name}";
    }

    private SqlParameter FromValue(object value,string param = null) => new(parameterName: FixName(param),value: value ?? DBNull.Value);

    private SqlParameter ObjectToJson(object value,string param = null)
    => new()
    {
      ParameterName = FixName(param),
      Value = JsonSerializer.Serialize(value),
      SqlDbType = SqlDbType.VarChar
    };

    private void UpdateCommand()
    {
      if(CommandTimeOut > 0)
      {
        Command.CommandTimeout = CommandTimeOut;
      }
    }

    #endregion Private Methods
  }
}
