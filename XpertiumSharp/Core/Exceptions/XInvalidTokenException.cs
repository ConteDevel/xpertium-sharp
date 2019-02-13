using System;
using System.Runtime.Serialization;

namespace XpertiumSharp.Core.Exceptions
{
    public class XInvalidTokenException : Exception
    {
        public XInvalidTokenException()
        {
        }

        public XInvalidTokenException(string message) : base(message)
        {
        }

        public XInvalidTokenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected XInvalidTokenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
