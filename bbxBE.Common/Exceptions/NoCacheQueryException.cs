using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Common.Exceptions
{
    public class NoCacheQueryException : Exception
    {
        public NoCacheQueryException() : base()
        {
        }

        public NoCacheQueryException(string message) : base(message)
        {
        }
    }
}
