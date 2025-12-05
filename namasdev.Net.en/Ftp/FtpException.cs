using System;
using System.Net;

using namasdev.Core.Validation;

namespace namasdev.Net.Ftp
{
    [Serializable]
    public class FtpException : Exception
    {
        public FtpException() { }
        public FtpException(string message) : base(message) { }
        public FtpException(string message, Exception inner) : base(message, inner) { }

        public FtpException(FtpStatusCode statusCode, string statusCodeDescription)
            : this(statusCodeDescription, statusCode, statusCodeDescription, null)
        {
        }

        public FtpException(string message, FtpStatusCode statusCode, string statusCodeDescription)
            :this(message, statusCode, statusCodeDescription, null)
        {
        }

        public FtpException(string message, FtpStatusCode statusCode, string statusCodeDescription, Exception inner)
            : this(message, inner)
        {
            Validator.ValidateRequiredArgumentAndThrow(statusCodeDescription, nameof(statusCodeDescription));

            StatusCode = StatusCode;
            StatusCodeDescription = statusCodeDescription;
        }

        protected FtpException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public FtpStatusCode StatusCode { get; private set; }
        public string StatusCodeDescription { get; private set; }
    }
}
