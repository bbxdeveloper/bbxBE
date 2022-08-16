using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbxBE.Common.Exceptions
{
    public class AuthException : Exception
    {
        public AuthException(string message) : base(message)
        {
        }
    }
}
