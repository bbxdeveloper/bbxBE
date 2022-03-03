using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Application.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(string message) : base(message) { }
  
    }
}
