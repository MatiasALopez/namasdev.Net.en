using System.Collections.Generic;
using System.Net;

namespace namasdev.Net.Mail
{
    public class EmailServerParameters
    {
        public EmailServerParameters()
        {
        }

        public EmailServerParameters(string host,
            int? port = null, NetworkCredential credentials = null, bool? enableSsl = null,
            string from = null, string bcc = null,
            IEnumerable<KeyValuePair<string, string>> headers = null,
            string pickupDirectory = null)
        {
            Host = host;
            Port = port;
            Credentials = credentials;
            EnableSsl = enableSsl;
            From = from;
            BCC = bcc;
            Headers = headers;
            PickupDirectory = pickupDirectory;
        }

        public string Host { get; set; }
        public int? Port { get; set; }
        public NetworkCredential Credentials { get; set; }
        public bool? EnableSsl { get; set; }
        public string From { get; set; }
        public string BCC { get; set; }
        public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }
        public string PickupDirectory { get; set; }
    }
}
