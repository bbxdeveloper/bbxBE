using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Application.Interfaces.Queries
{
    public interface IQueryParameter : IPagingParameter
    {
        public string OrderBy { get; set; }
   //     public string Fields { get; set; }
    }
}
