using System;

using namasdev.Core.Validation;

namespace namasdev.Net.Ftp
{
    public class FtpDirectory : IFtpEntry
    {
        private string _name;

        public FtpDirectory(Uri uri, IFtpEntry[] entries)
        {
            Validator.ValidateRequiredArgumentAndThrow(uri, nameof(uri));

            Uri = uri;
            Entries = entries ?? new IFtpEntry[0];
        }

        public string Name
        {
            get { return _name ?? (_name = FtpEntryHelper.GetDirectoryNameFromUri(Uri)); }
        }

        public Uri Uri { get; private set; }
        public IFtpEntry[] Entries { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
