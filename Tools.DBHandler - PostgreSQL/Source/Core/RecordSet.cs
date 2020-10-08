using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Tools.DBHandler
{
  public class RecordSet:List<Record>
  {
    #region Public Methods

    public void AddObject(object obj) => Add(Record.FromObject(obj));

    public IEnumerable<T> Cast<T>(Func<Record,T> func) => this.Select(c => func(c)).ToArray();

    public IEnumerable<T> Cast<T>() where T : class => this.Select(c => c.CastObj<T>()).ToArray();

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Redundancy","RCS1163:Unused parameter.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style","IDE0060:Remove unused parameter")]
    public IEnumerable<T> Cast<T>(T anonymousTypeObject) where T : class => Cast<T>();

    public IEnumerable<T> CastJson<T>() where T : class => JsonSerializer.Deserialize<T[]>(JsonSerializer.Serialize(this));

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Redundancy","RCS1163:Unused parameter.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style","IDE0060:Remove unused parameter")]
    public IEnumerable<T> CastJson<T>(T anonymousTypeObject) where T : class => CastJson<T>();

    public IEnumerable<object> CastJson(Type type) => this.Select(c =>  JsonSerializer.Deserialize(JsonSerializer.Serialize(c),type)).ToArray();

    public string ToJson() => JsonSerializer.Serialize(this);

    #endregion Public Methods
  }
}
