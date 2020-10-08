//using System;

//namespace Tools.DBHandler
//{
//  public class EntityBuilder<T> where T : IEntity
//  {
//    #region Private Fields
//    private static readonly Type type = typeof(T);
//    private DBHandler<object> dBHandler;
//    #endregion Private Fields

//    #region Public Constructors

//    public EntityBuilder(DBHandler<object> dBHandler)
//    {
//      this.dBHandler = dBHandler;
//    }

//    #endregion Public Constructors

//    #region Public Methods

//    public EntityBuilder<T> AssignCast(Func<Record,T> func)
//    {
//      _Sql_Cast[type] = func;
//      return this;
//    }

//    public EntityBuilder<T> AssignDelete(Func<T,int> func)
//    {
//      _Sql_Delete[type] = new Func<IEntity,int>(x => func((T)x));
//      return this;
//    }

//    public EntityBuilder<T> AssignInsert(Func<T,int> func)
//    {
//      _Sql_Insert[type] = new Func<IEntity,int>(x => func((T)x));
//      return this;
//    }

//    public EntityBuilder<T> AssignUpdate(Func<T,int> func)
//    {
//      _Sql_Delete[type] = new Func<IEntity,int>(x => func((T)x));
//      return this;
//    }

//    #endregion Public Methods
//  }
//}
