using System;
using System.Globalization;

namespace bbxBE.Common.Exceptions
{
    public class NAVException : Exception
    {
        public NAVException() : base()
        {
        }

        public NAVException(string message) : base(message)
        {
        }

        public NAVException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }

        public NAVException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}