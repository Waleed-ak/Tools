using System;
using System.Text.Json;

namespace Tools.Logger
{
  public class LogValue
  {
    #region Private Fields

    private static readonly JsonSerializerOptions _JsonSerializerOptions = new JsonSerializerOptions
    {
      IgnoreNullValues = true,
      IgnoreReadOnlyProperties = false,
      WriteIndented = true,
    };

    #endregion Private Fields

    #region Public Constructors

    public LogValue()
    {
      Machine = Environment.MachineName;
      Time = DateTime.Now;
    }

    public LogValue(string application,string className,string methodName,string action,string key,object value,string note,object obj,string message,Exception exception,string version)
    {
      Action = action;
      Application = application;
      Class = className;
      Exception = LocalGetDescription(exception);
      JsonObj = ToJson(obj);
      Key = key;
      Machine = Environment.MachineName;
      Message = message;
      Method = methodName;
      Note = note;
      Time = DateTime.Now;
      TypeObj = LocalGetName(obj);
      Value = ToJson(value);
      Version = version;
    }

    #endregion Public Constructors

    #region Public Properties
    public string Action { get; set; }
    public string Application { get; set; }
    public string Class { get; set; }
    public string Exception { get; set; }
    public string JsonObj { get; set; }
    public string Key { get; set; }
    public string Machine { get; set; }
    public string Message { get; set; }
    public string Method { get; set; }
    public string Note { get; set; }
    public DateTime Time { get; set; }
    public string TypeObj { get; set; }
    public string Value { get; set; }
    public string Version { get; set; }
    #endregion Public Properties

    #region Public Methods

    public string GetText()
    {
      var text = $"---------------- {Time:HH:mm:ss}";
      text += $"\n--Machine: {Machine} / Action: {Action}";
      if(Class != null)
      {
        text += $"\n--Application: {Application} Version: {Version}    Class: {Class}  /  Method: {Method}";
      }
      if(Note != null)
      {
        text += $"\n--Note\n{Note}";
      }
      if(Key != null)
      {
        text += $"\n--Key: {Key}  Value: {Value}";
      }
      if(JsonObj != null)
      {
        text += $"\n--{TypeObj}\n{JsonObj}";
      }
      if(Message != null)
      {
        text += $"\n--Message: {Message}";
      }
      if(Exception != null)
      {
        text += $"\n--Exception\n{Exception}";
      }
      text += "\n----------------\n";
      return text;
    }

    #endregion Public Methods

    #region Private Methods

    private static string LocalGetDescription(Exception exception) => exception?.GetDescription();

    private static string LocalGetName(object obj) => obj?.GetType().GetFriendlyName();

    private string ToJson(object value)
    {
      if(value != null)
      {
        try
        {
          return JsonSerializer.Serialize(value,_JsonSerializerOptions);
        }
        catch(JsonException ex)
        {
          return "Error Serializing \n" + ex.GetDescription();
        };
      }
      else
      {
        return null;
      }
    }

    #endregion Private Methods
  }
}
