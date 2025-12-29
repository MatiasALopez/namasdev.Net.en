using System.Net.Mail;

namespace namasdev.Net.Mail
{
    public interface IEmailServer
    {
        void SendMail(MailMessage mail);
    }
}
