using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbxBE.Common.Attributes
{
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }

        public TableAttribute(string p_name)
        {
            Name = p_name;
        }
    }
}
