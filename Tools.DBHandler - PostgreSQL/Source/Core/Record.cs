using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using static System.Text.Json.JsonSerializer;

namespace Tools.DBHandler
{
  public static class RecordExt
  {
    #region Public Methods

    public static IEnumerable<T> DoAction<T>(this IEnumerable<T> items,Action<T> action) where T : class
    {
      foreach(var item in items)
      {
        action(item);
        yield return item;
      }
    }

    public static IEnumerable<Record> ToCsvFile(this IEnumerable<Record> items,string fileName,string header,string sperator = ",",int bulkCount = 50000)
    {
      var stringBuilder = new StringBuilder();
      var count = 1;
      using var fw = new StreamWriter(fileName);
      if(!string.IsNullOrWhiteSpace(header))
      {
        fw.WriteLine(header.Trim());
      }
      foreach(var item in items)
      {
        var line = string.Join(sperator,item.Select(c => c.Value?.ToString()));
        stringBuilder.AppendLine(line);

        if(++count > bulkCount)
        {
          fw.Write(stringBuilder);
          fw.Flush();
          count = 1;
          stringBuilder.Clear();
        }
        yield return item;
      }
      fw.Write(stringBuilder);
      fw.Flush();
      fw.Close();
    }

    #endregion Public Methods
  }

  [SuppressMessage("Design","CA1031:Do not catch general exception types")]
  public class Record:Dictionary<string,object>
  {
    #region Public Constructors

    public Record()
    {
    }

    public Record(object obj)
    {
      if(obj is string str)
      {
        var root = JsonDocument.Parse(str)?.RootElement;
        if(root is not null && root.Value.ValueKind == JsonValueKind.Object)
        {
          foreach(var item in root.Value.EnumerateObject())
          {
            this[item.Name] = item.Value;
          }
        }
      }

      foreach(var item in obj.GetType().GetProperties())
      {
        this[item.Name] = item.GetValue(obj);
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public static Record FromDictionary(IDictionary<string,object> pairs) => new Record().Merge(pairs);

    public static Record FromJson(string json) => new(json);

    public static Record FromObject<T>(T obj) where T : class => new(obj);

    [SuppressMessage("Redundancy","RCS1163:Unused parameter.")]
    [SuppressMessage("Style","IDE0060:Remove unused parameter")]
    public T Cast<T>(T anonymousTypeObject) where T : class => Cast<T>();

    public object Cast(Type type) => ConvertAny.Cast(this,type);

    public T Cast<T>() => ConvertAny.Cast<T>(this);

    public T Cast<T>(Func<Record,T> func) => func(this);

    [SuppressMessage("Redundancy","RCS1163:Unused parameter.")]
    [SuppressMessage("Style","IDE0060:Remove unused parameter")]
    public T CastObj<T>(T anonymousTypeObject) where T : class => CastObj<T>();

    public T CastObj<T>()
    {
      var TType = typeof(T);
      if(!TType.IsClass && ConvertAny.TryTo<T>(this.FirstOrDefault().Value,out var res))
      {
        return res;
      }
      var result = (T)Activator.CreateInstance(TType);
      foreach(var item in this)
      {
        SetProperty(item);
      }
      return result;
      static object Parse(PropertyInfo info,object obj)
      {
        if(info.PropertyType.Name == "String")
          return obj.ToString();
        return info.PropertyType.GetMethod("Parse",new[] { typeof(string) })?.Invoke(null,new[] { obj.ToString() }) ?? throw new NotSupportedException();
      }
      void SetProperty(KeyValuePair<string,object> item)
      => _ = TType
          .GetProperty(item.Key)?
          .With(c => c.SetValue(result,Parse(c,item.Value)));
    }

    public Record Merge(IDictionary<string,object> dictionary)
    {
      foreach(var item in dictionary)
      {
        this[item.Key] = item.Value;
      }
      return this;
    }

    public T To<T>(string key,T defaultVal = default)
    {
      try
      {
        if(!TryGetValue(key,out var obj))
          return defaultVal;
        return ConvertAny.To(obj,defaultVal);
      }
      catch { }
      return defaultVal;
    }

    public T To<T>(string key,Func<object,T> cast)
    {
      try
      {
        if(!TryGetValue(key,out var obj))
          return default;
        if(obj is null || obj == DBNull.Value)
          obj = null;
        return cast(obj);
      }
      catch { }
      return default;
    }

    public bool ToBool(string key,bool defaultVal = false)
    {
      try
      { return Convert.ToBoolean(TryGetValue(key,out var value) ? value : defaultVal); }
      catch { }
      return defaultVal;
    }

    public byte[] ToBytes(string key,byte[] defaultVal = default)
    {
      defaultVal ??= Array.Empty<byte>();
      try
      {
        if(TryGetValue(key,out var value))
        {
          if(value is null)
            return defaultVal;
          if(value is byte[] bytes)
            return bytes;
          return Deserialize<byte[]>(Serialize(value)) ?? defaultVal;
        }
      }
      catch { }
      return defaultVal;
    }

    public IDictionary<string,string> ToDic() => this.ToDictionary(d => d.Key,d => d.Value?.ToString());

    public string ToJson(string key = null,object defaultVal = null)
    {
      if(key is null)
      {
        return Serialize(this);
      }
      try
      {
        return Serialize(TryGetValue(key,out var value) ? value : defaultVal);
      }
      catch { }
      return Serialize(defaultVal);
    }

    public bool TryTo<T>(string key,out T value)
    {
      value = default;
      try
      {
        if(!TryGetValue(key,out var obj))
          return false;
        if(ConvertAny.TryTo<T>(obj,out var res))
        {
          value = res;
          return true;
        }

        return false;
      }
      catch
      {
        return false;
      }
    }

    public bool TryTo<T>(string key,Func<object,T> cast,out T value)
    {
      value = default;
      try
      {
        if(!TryGetValue(key,out var obj))
          return false;
        if(obj is null || obj == DBNull.Value)
          obj = null;
        value = cast(obj);
        return true;
      }
      catch
      {
        return false;
      }
    }

    #endregion Public Methods
  }
}
