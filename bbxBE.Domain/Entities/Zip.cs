using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace bbxBE.Domain.Entities
{
    [Description("Irányítószám")]
    public class Zip : BaseEntity
    {
        [ColumnLabel("Irányítószám")]
        [Description("Irányítószám")]
        public string ZipCode { get; set; }

        [ColumnLabel("Város")]
        [Description("Város")]
        public string ZipCity { get; set; }

        [ColumnLabel("Megye")]
        [Description("Megye")]
        public string ZipCounty { get; set; }


    }
}
