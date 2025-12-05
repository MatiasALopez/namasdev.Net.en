using System.Net.Mail;

namespace namasdev.Net.Correos
{
    public interface IMailServer
    {
        void SendMail(MailMessage mail);
    }
}
