using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Common.Exceptions
{
    public class LockedCacheException : Exception
    {
        public LockedCacheException() : base()
        {
        }

        public LockedCacheException(string message) : base(message)
        {
        }
    }
}
