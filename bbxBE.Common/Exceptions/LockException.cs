using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Common.Exceptions
{
    public class LockException : Exception
    {
        public LockException() : base()
        {
        }

        public LockException(string message) : base(message)
        {
        }
    }
}
