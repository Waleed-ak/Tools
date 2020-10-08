using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using static System.Text.Json.JsonSerializer;

namespace Tools.DBHandler
{
  public static class ConvertAny
  {
    #region Private Fields

    private static readonly Dictionary<Type,Func<object,object,object>> ConvertDic = new()
    {
      [typeof(bool)] = ConTypeBool,
      [typeof(byte)] = ConTypeByte,
      [typeof(byte[])] = ConTypeBytes,
      [typeof(DateTime)] = ConTypeDateTime,
      [typeof(decimal)] = ConTypeDecimal,
      [typeof(double)] = ConTypeDouble,
      [typeof(float)] = ConTypeFloat,
      [typeof(Guid)] = ConTypeGuid,
      [typeof(int)] = ConTypeInt,
      [typeof(long)] = ConTypeLong,
      [typeof(sbyte)] = ConTypeSByte,
      [typeof(short)] = ConTypeShort,
      [typeof(string)] = ConTypeString,
      [typeof(uint)] = ConTypeUInt,
      [typeof(ulong)] = ConTypeULong,
      [typeof(ushort)] = ConTypeUShort,
    };

    private static readonly JsonSerializerOptions JsonDesOtions = new()
    {
      //NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    private static readonly Type[] NumberType = {
      typeof(byte),
      typeof(decimal),
      typeof(double),
      typeof(float),
      typeof(int),
      typeof(long),
      typeof(sbyte),
      typeof(short),
      typeof(uint),
      typeof(ulong),
      typeof(ushort),
    };

    private static readonly Encoding UTF8 = new UTF8Encoding(true,true);
    #endregion Private Fields

    #region Public Methods

    public static object Cast(object obj,Type type) => Deserialize(Serialize(obj),type);

    public static T Cast<T>(Record record)
    {
      var TType = typeof(T);
      if(ConvertDic.TryGetValue(TType,out var converter))
      {
        return (T)converter(record.FirstOrDefault().Value,default(T));
      }
      if(TType.IsEnum)
      {
        var fv = record.FirstOrDefault().Value;
        return fv is string str ? (T)Enum.Parse(TType,str.Trim(),true) : (T)Enum.ToObject(TType,fv);
      }
      return Deserialize<T>(Serialize(record));
    }

    [SuppressMessage("Design","CA1031:Do not catch general exception types")]
    public static T To<T>(object item,T defaultVal = default)
    {
      if(item is null || item == DBNull.Value)
        return defaultVal;
      if(item is T val)
        return val;
      try
      {
        var convertType = typeof(T);
        if(ConvertDic.TryGetValue(convertType,out var converter))
          return (T)converter(item,defaultVal);
        if(convertType.IsEnum)
          return item is string str ? (T)Enum.Parse(convertType,str.Trim(),true) : (T)Enum.ToObject(convertType,item);
        if(convertType.IsClass)
        {
          return (T)Deserialize(item switch
          {
            string str => str,
            byte[] bytes => UTF8.GetString(bytes),
            _ => Serialize(item),
          },convertType,JsonDesOtions);
        }
        return defaultVal;
      }
      catch
      {
        return defaultVal;
      }
    }

    [SuppressMessage("Design","CA1031:Do not catch general exception types")]
    public static bool TryTo<T>(object item,out T result,T defaultVal = default)
    {
      if(item is null || item == DBNull.Value)
      {
        result = defaultVal;
        return true;
      }
      if(item is T val)
      {
        result = val;
        return true;
      }
      result = defaultVal;
      try
      {
        var convertType = typeof(T);
        if(ConvertDic.TryGetValue(convertType,out var converter))
        {
          result = (T)converter(item,defaultVal);
          return true;
        }
        if(convertType.IsEnum)
        {
          result = item is string str ? (T)Enum.Parse(convertType,str.Trim(),true) : (T)Enum.ToObject(convertType,item);
          return true;
        }
        if(convertType.IsClass)
        {
          result = (T)Deserialize(item switch
          {
            string str => str,
            byte[] bytes => UTF8.GetString(bytes),
            _ => Serialize(item),
          },convertType,JsonDesOtions);
          return true;
        }
        return false;
      }
      catch
      {
        return false;
      }
    }

    #endregion Public Methods

    #region Private Methods

    private static object ConTypeBool(object c,object d) => c switch
    {
      string str => bool.TryParse(str,out var res) ? res : d,
      var i when i.GetType().In(NumberType) => (float)i != 0,
      _ => d,
    };

    private static object ConTypeByte(object c,object d) => c switch
    {
      string str => byte.TryParse(str,out var res) ? res : d,
      var i when i.GetType().In(NumberType) => (byte)i,
      _ => d,
    };

    private static object ConTypeBytes(object c,object d) => c switch
    {
      byte[] bytes => bytes,
      string str => UTF8.GetBytes(str),
      var i when i.GetType().IsClass => UTF8.GetBytes(Serialize(i)),
      _ => UTF8.GetBytes(c.ToString()),
    };

    private static object ConTypeDateTime(object c,object d) => c switch
    {
      string str => DateTime.TryParse(str,out var res) ? res : d,
      _ => d,
    };

    private static object ConTypeDecimal(object c,object d) => c switch
    {
      string str => decimal.TryParse(str,out var res) ? res : d,
      var i when i.GetType().In(NumberType) => (decimal)i,
      _ => d,
    };

    private static object ConTypeDouble(object c,object d) => c switch
    {
      string str => double.TryParse(str,out var res) ? res : d,
      var i when i.GetType().In(NumberType) => (double)i,
      _ => d,
    };

    private static object ConTypeFloat(object c,object d) => c switch
    {
      string str => float.TryParse(str,out var res) ? res : d,
      var i when i.GetType().In(NumberType) => (float)i,
      _ => d,
    };

    private static object ConTypeGuid(object c,object d) => c switch
    {
      string str => Guid.TryParse(str,out var res) ? res : d,
      _ => d,
    };

    private static object ConTypeInt(object c,object d) => c switch
    {
      string str => int.TryParse(str,out var res) ? res : d,
      var i when i.GetType().In(NumberType) => (int)i,
      _ => d
    };

    private static object ConTypeLong(object c,object d) => c switch
    {
      string str => long.TryParse(str,out var res) ? res : d,
      var i when i.GetType().In(NumberType) => (long)i,
      _ => d,
    };

    private static object ConTypeSByte(object c,object d) => c switch
    {
      string str => sbyte.TryParse(str,out var res) ? res : d,
      var i when i.GetType().In(NumberType) => (sbyte)i,
      _ => d,
    };

    private static object ConTypeShort(object c,object d) => c switch
    {
      string str => short.TryParse(str,out var res) ? res : d,
      var i when i.GetType().In(NumberType) => (short)i,
      _ => d,
    };

    private static object ConTypeString(object c,object d) => c switch
    {
      byte[] bytes => UTF8.GetString(bytes),
      string str => str,
      var i when i.GetType().IsClass => Serialize(i),
      _ => c.ToString(),
    };

    private static object ConTypeUInt(object c,object d) => c switch
    {
      string str => uint.TryParse(str,out var res) ? res : d,
      var i when i.GetType().In(NumberType) => (uint)i,
      _ => (uint)c,
    };

    private static object ConTypeULong(object c,object d) => c switch
    {
      string str => ulong.TryParse(str,out var res) ? res : d,
      var i when i.GetType().In(NumberType) => (ulong)i,
      _ => (ulong)c,
    };

    private static object ConTypeUShort(object c,object d) => c switch
    {
      string str => ushort.TryParse(str,out var res) ? res : d,
      var i when i.GetType().In(NumberType) => (ushort)i,
      _ => (short)c,
    };

    #endregion Private Methods
  }
}
