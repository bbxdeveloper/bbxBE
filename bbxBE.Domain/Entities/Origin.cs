using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace bbxBE.Domain.Entities
{
    [Description("Származási hely")]
    public class Origin : BaseEntity
    {
        [ColumnLabel("Kód")]
        [Description("Kód")]
        public string OriginCode { get; set; }
        [ColumnLabel("Leírás")]
        [Description("Leírás")]
        public string OriginDescription { get; set; }


    }
}
