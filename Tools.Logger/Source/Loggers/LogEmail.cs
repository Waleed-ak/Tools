using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Tools.Logger
{
  public class LogEmail:LogItem, ILogItem
  {
    #region Private Fields
    private readonly ICredentialsByHost Credentials;
    private readonly bool EnableSsl;
    private readonly MailAddress FromAddress;
    private readonly string Host;
    private readonly string MailAddresses;
    private readonly int Port;
    private readonly bool UseDefaultCredentials;
    #endregion Private Fields

    #region Public Constructors

    public LogEmail(LogEmailSettings setting)
    {
      ApplySettings(setting);
      EnableSsl = setting.EnableSsl;
      Host = setting.Host;
      Port = setting.Port;
      UseDefaultCredentials = setting.UseDefaultCredentials;
      if(UseDefaultCredentials)
      {
        Credentials = new NetworkCredential(userName: setting.UserName,password: setting.Password);
      }
      MailAddresses = setting.MailAddresses;
      if(setting.FromAddress != null)
      {
        FromAddress = new MailAddress(setting.FromAddress);
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public void Log(LogValue value)
    {
      var text = $"<html><body><style>h3{{background-color:#eef;}}</style><h3>{value.Time:yyyy-MM-dd HH:mm:ss}</h3>";
      text += $"<h3>Machine: {value.Machine} / Action: {value.Action}</h3>";
      if(value.Class != null)
      {
        text += $"<h3>Class {value.Class} / Method {value.Method}</h3>";
      }
      if(value.Note != null)
      {
        text += $"<h3>Note</h3><p>{value.Note}</p>";
      }
      if(value.Key != null)
      {
        text += $"<h3>Key {value.Key}  Value {value.Value}</h3>";
      }
      if(value.JsonObj != null)
      {
        text += $"<h3>{value.TypeObj}</h3><code>{value.JsonObj}</code>";
      }
      if(value.Message != null)
      {
        text += $"<h3>{value.Message}</h3>";
      }
      if(value.Exception != null)
      {
        text += $"<code>{value.Exception}</p>";
      }
      text += "</body></html>";
      SendEmail($"[{value.Machine}] Alert: {value.Application} ({value.Version}) - {value.Action}",text).GetAwaiter().GetResult();
    }

    #endregion Public Methods

    #region Private Methods

    private MailMessage GetMailMessage(string emailSubject,string emailMessage)
    {
      var message = new MailMessage
      {
        Subject = emailSubject,
        Body = emailMessage.Replace("\n","<br/>").Replace(" ","&nbsp;"),
        IsBodyHtml = true,
        From = FromAddress,
      };
      message.Bcc.Add(MailAddresses);
      return message;
    }

    private SmtpClient GetSmtpClient() => new SmtpClient()
    {
      Host = Host,
      Port = Port,
      EnableSsl = EnableSsl,
      UseDefaultCredentials = UseDefaultCredentials,
      Credentials = Credentials,
    };

    private async Task SendEmail(string emailSubject,string emailMessage)
    {
      using(var message = GetMailMessage(emailSubject,emailMessage))
      using(var smtpClient = GetSmtpClient())
      {
        await smtpClient.SendMailAsync(message).ConfigureAwait(false);
      }
    }

    #endregion Private Methods
  }
}
