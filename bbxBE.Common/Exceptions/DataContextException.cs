using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Common.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(string message) : base(message) { }
  
    }
}
