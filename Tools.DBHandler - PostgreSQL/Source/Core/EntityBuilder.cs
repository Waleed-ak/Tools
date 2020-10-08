using System;
using System.Collections.Generic;

namespace Tools.DBHandler
{
  public class EntityBuilder<DB, T>
    where DB : DBHandler<DB>
    where T : IEntity
  {
    #region Private Fields
    private readonly DBHandler<DB> DBHandler;
    #endregion Private Fields

    #region Public Constructors

    public EntityBuilder(DBHandler<DB> dBHandler)
    {
      DBHandler = dBHandler;
    }

    #endregion Public Constructors

    #region Public Methods

    public EntityBuilder<DB,T> AssignCast(Func<Record,T> func)
    {
      DBHandler.AssignCastFunc(func);
      return this;
    }

    public EntityBuilder<DB,T> AssignDelete(Func<T,int> func)
    {
      DBHandler.AssignDelete(func);
      return this;
    }

    public EntityBuilder<DB,T> AssignDeleteBulk(Action<IEnumerable<T>> action)
    {
      DBHandler.AssignDeleteBulk(action);
      return this;
    }

    public EntityBuilder<DB,T> AssignInsert(Func<T,int> func)
    {
      DBHandler.AssignInsert(func);
      return this;
    }

    public EntityBuilder<DB,T> AssignInsertBulk(Action<IEnumerable<T>> action)
    {
      DBHandler.AssignInsertBulk(action);
      return this;
    }

    public EntityBuilder<DB,T> AssignUpdate(Func<T,int> func)
    {
      DBHandler.AssignUpdate(func);
      return this;
    }

    public EntityBuilder<DB,T> AssignUpdateBulk(Action<IEnumerable<T>> action)
    {
      DBHandler.AssignUpdateBulk(action);
      return this;
    }

    #endregion Public Methods
  }
}
