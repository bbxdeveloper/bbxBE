using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace bbxBE.Domain.Entities
{
    public class Counter : BaseEntity
    {
        public string CounterCode { get; set; }
        public string CounterDescription { get; set; }
        public long? WarehouseID { get; set; }
        public string Prefix { get; set; }
        public long CurrentNumber { get; set; }
        public long NumbepartLength { get; set; }
        public string Suffix { get; set; }

        [ForeignKey("WarehouseID")]
        public Warehouse Warehouse { get; set; }
    }
}
