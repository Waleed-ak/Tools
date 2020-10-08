using System;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace Tools.Logger
{
  public class LogSql:LogItem, ILogItem
  {
    #region Private Fields
    private static readonly object _Lock = new object();
    private static readonly string Machine = LocalMachine();
    private static int Counter;
    private readonly string CommandStr;
    private readonly string ConnectionStr;
    #endregion Private Fields

    #region Public Constructors

    public LogSql(LogSqlSettings setting)
    {
      ApplySettings(setting);
      ConnectionStr = setting.ConnectionStr;
      CommandStr = $"Insert Into {setting.TableName}(Id,[Application],[Action],[Machine],[Class],[Method],[Time],[Key],[Value],[Note],[TypeObj],[JsonObj],[Message],[Exception],[Version]) Values(@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15);";
    }

    #endregion Public Constructors

    #region Public Methods

    [SuppressMessage("Design","CA1031")]
    public void Log(LogValue value)
    {
      try
      {
        LocalLog(value);
      }
      catch(Exception ex)
      {
        LocalLog(value.Continue(c =>
        {
          c.Exception = "Sql Error\n" + ex.GetDescription();
          c.JsonObj = null;
          c.Message = null;
          c.Note = null;
          c.TypeObj = null;
          c.Value = null;
        }));
      }
    }

    #endregion Public Methods

    #region Private Methods

    private static string LocalMachine()
    {
      var hash = Environment.MachineName.GetHashCode();
      hash = 0xffff & (hash ^ hash >> 16);
      return $"{hash:X4}";
    }

    private static Guid LocalSqlTimeGuid(DateTime date)
    {
      lock(_Lock)
      {
        Counter++;
        Counter %= 0X1000000;
        return Guid.Parse($"{Counter:X8}{Machine}{date:ffffss00yyyyMMddHHmm}");
      }
    }

    [SuppressMessage("Security","CA2100")]
    private void LocalLog(LogValue value)
    {
      using var connection = new SqlConnection(ConnectionStr);
      using var command = new SqlCommand
      {
        CommandText = CommandStr,
        Connection = connection,
        CommandType = System.Data.CommandType.Text
      };
      var par = command.Parameters;
      par.AddWithValue("@1",LocalSqlTimeGuid(value.Time));
      par.AddWithValue("@2",(object)value.Application ?? DBNull.Value);
      par.AddWithValue("@3",(object)value.Action ?? DBNull.Value);
      par.AddWithValue("@4",(object)value.Machine ?? DBNull.Value);
      par.AddWithValue("@5",(object)value.Class ?? DBNull.Value);
      par.AddWithValue("@6",(object)value.Method ?? DBNull.Value);
      par.AddWithValue("@7",(object)value.Time ?? DBNull.Value);
      par.AddWithValue("@8",(object)value.Key ?? DBNull.Value);
      par.AddWithValue("@9",(object)value.Value ?? DBNull.Value);
      par.AddWithValue("@10",(object)value.Note ?? DBNull.Value);
      par.AddWithValue("@11",(object)value.TypeObj ?? DBNull.Value);
      par.AddWithValue("@12",(object)value.JsonObj ?? DBNull.Value);
      par.AddWithValue("@13",(object)value.Message ?? DBNull.Value);
      par.AddWithValue("@14",(object)value.Exception ?? DBNull.Value);
      par.AddWithValue("@15",(object)value.Version ?? DBNull.Value);
      connection.Open();
      command.ExecuteNonQuery();
      connection.Close();
    }

    #endregion Private Methods
  }
}
