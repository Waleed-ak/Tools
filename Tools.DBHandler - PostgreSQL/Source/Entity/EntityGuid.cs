using System;

namespace Tools.DBHandler
{
  public abstract class EntityGuid:IEntity<Guid>
  {
    #region Public Properties
    public Guid Id { get; set; } = GuidTools.GetGuid();
    #endregion Public Properties
  }
}
