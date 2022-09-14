using System;
using System.Globalization;

namespace bbxBE.Common.Exceptions
{
    public class CacheException : Exception
    {
        public CacheException() : base()
        {
        }

        public CacheException(string message) : base(message)
        {
        }

        public CacheException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }

        public CacheException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}