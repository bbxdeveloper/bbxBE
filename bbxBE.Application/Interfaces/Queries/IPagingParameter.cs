using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Application.Interfaces.Queries
{
    public  interface IPagingParameter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
