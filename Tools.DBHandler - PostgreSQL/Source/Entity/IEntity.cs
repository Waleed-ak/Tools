namespace Tools.DBHandler
{
  public interface IEntity
  {
  }

  public interface IEntity<T>:IEntity
  {
    #region Public Properties
    T Id { get; set; }
    #endregion Public Properties
  }
}
