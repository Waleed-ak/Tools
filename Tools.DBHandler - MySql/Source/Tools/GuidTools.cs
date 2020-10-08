using System;

namespace Tools.DBHandler
{
  public static class GuidTools
  {
    #region Private Fields
    private static readonly object Lock = new object();
    private static readonly string Machine = LocalMachine();
    private static uint Seq4bytes;
    #endregion Private Fields

    #region Public Methods

    public static Guid GetGuid(DateTime dt = default)
    {
      lock(Lock)
      {
        if(dt == default)
        {
          dt = DateTime.Now;
        }
        return new Guid($"{Machine}{Reverse():X8}{dt:ssffyyyyMMddHHmm}");
      }
    }

    #endregion Public Methods

    #region Private Methods

    private static string LocalMachine() => $"{ Environment.MachineName.GetHashCode():X8}";

    private static uint Reverse()
    {
      Seq4bytes++;
      return (Seq4bytes & 0x000000FFU) << 24 | (Seq4bytes & 0x0000FF00U) << 8 | (Seq4bytes & 0x00FF0000U) >> 8 | (Seq4bytes & 0xFF000000U) >> 24;
    }

    #endregion Private Methods
  }
}
