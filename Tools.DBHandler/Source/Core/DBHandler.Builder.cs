namespace Tools.DBHandler
{
  public class Builder<DB> where DB : DBHandler<DB>
  {
    #region Public Constructors

    public Builder(DBHandler<DB> dBHandler)
    {
      DBHandler = dBHandler;
    }

    #endregion Public Constructors

    #region Public Properties
    public DBHandler<DB> DBHandler { get; }
    #endregion Public Properties

    #region Public Methods

    public EntityBuilder<DB,T> Of<T>() where T : IEntity => new(DBHandler);

    #endregion Public Methods
  }
}
