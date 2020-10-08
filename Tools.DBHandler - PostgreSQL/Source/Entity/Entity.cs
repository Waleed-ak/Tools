namespace Tools.DBHandler
{
  public abstract class Entity<T>:IEntity<T>
  {
    #region Public Properties
    public T Id { get; set; }
    #endregion Public Properties
  }
}
