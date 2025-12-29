using System;
using System.Net.Mail;

using namasdev.Core.Validation;

namespace namasdev.Net.Mail
{
    public class EmailServer : IEmailServer, IDisposable
    {
        private SmtpClient _smtpClient;

        public EmailServer(EmailServerParameters parameters)
        {
            Parameters = parameters;

            BuildSmtpClient();
        }

        public EmailServerParameters Parameters { get; private set; }
        
        private void BuildSmtpClient()
        {
            if (!string.IsNullOrWhiteSpace(Parameters.PickupDirectory))
            {
                _smtpClient = new SmtpClient
                {
                    DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                    PickupDirectoryLocation = Parameters.PickupDirectory
                };
            }
            else
            {
                _smtpClient = new SmtpClient(Parameters.Host);

                if (Parameters.Port.HasValue)
                {
                    _smtpClient.Port = Parameters.Port.Value;
                }

                if (Parameters.Credentials != null)
                {
                    _smtpClient.Credentials = Parameters.Credentials;
                }

                if (Parameters.EnableSsl.HasValue)
                {
                    _smtpClient.EnableSsl = Parameters.EnableSsl.Value;
                }
            }
        }

        public void SendMail(MailMessage mail)
        {
            Validator.ValidateRequiredArgumentAndThrow(mail, nameof(mail));

            SetFrom(mail);
            SetHeaders(mail);
            SetBCC(mail);

            _smtpClient.Send(mail);
        }

        private void SetFrom(MailMessage mail)
        {
            if (mail.From == null
                && !string.IsNullOrWhiteSpace(Parameters.From))
            {
                mail.From = new MailAddress(Parameters.From);
            }
        }

        private void SetHeaders(MailMessage mail)
        {
            if (Parameters.Headers != null)
            {
                foreach (var header in Parameters.Headers)
                {
                    mail.Headers.Add(header.Key, header.Value);
                }
            }
        }

        private void SetBCC(MailMessage mail)
        {
            if (!string.IsNullOrWhiteSpace(Parameters.BCC))
            {
                mail.Bcc.Add(Parameters.BCC);
            }
        }

        public void Dispose()
        {
            if (_smtpClient != null)
            {
                _smtpClient.Dispose();
            }
        }
    }
}
