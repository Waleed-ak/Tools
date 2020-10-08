namespace Tools.Logger
{
  public class LogEmailSettings:LogItem
  {
    #region Public Properties
    public bool EnableSsl { get; set; }
    public string FromAddress { get; set; } = "no-reply@falconinsgroup.com";
    public string Host { get; set; }
    public string MailAddresses { get; set; }
    public string Password { get; set; }
    public int Port { get; set; } = 25;
    public bool UseDefaultCredentials { get; set; } = true;
    public string UserName { get; set; } = "no-reply@falconinsgroup.com";
    #endregion Public Properties
  }
}
