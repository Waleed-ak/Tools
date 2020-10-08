using System;
using MySql.Data.MySqlClient;

namespace Tools.DBHandler
{
  public interface IParams
  {
    #region Public Methods

    IParams Add(object value,string param = null);

    IParams AddJson(object value,string param = null);

    IParams AddSqlParam(MySqlParameter value,string param = null);

    IParams AddValues(params object[] values);

    IParams SetLog(Action<string> log);

    IParams SetTimeOut(int value);

    #endregion Public Methods
  }
}
