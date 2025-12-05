using System;

using namasdev.Core.Validation;

namespace namasdev.Net.Ftp
{
    public class FtpFile : IFtpEntry
    {
        private string _name;

        public FtpFile(Uri uri)
        {
            Validator.ValidateRequiredArgumentAndThrow(uri, nameof(uri));

            Uri = uri;
        }

        public string Name
        {
            get { return _name ?? (_name = FtpEntryHelper.GetFileNameFromUri(Uri)); } 
        }

        public Uri Uri { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
