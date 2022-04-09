using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbxBE.Common.Attributes
{
    public class ColumnLabelAttribute : Attribute
    {
        public string LabelText { get; set; }

        public ColumnLabelAttribute(string _labelText)
        {
            LabelText = _labelText;
        }
    }
}
