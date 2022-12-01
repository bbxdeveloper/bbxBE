using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace bbxBE.Domain.Entities
{
    [Description("Hely")]
    public class Location : BaseEntity
    {
        [ColumnLabel("Kód")]
        [Description("Kód")]
        public string LocationCode { get; set; }

        [ColumnLabel("Leírás")]
        [Description("Leírás")]
        public string LocationDescription { get; set; }


    }
}
