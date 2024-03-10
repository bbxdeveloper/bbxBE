﻿using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace bbxBE.Domain.Entities
{
    [Description("Bizonylati tömb")]
    public class Counter : BaseEntity
    {
        [ColumnLabel("Kód")]
        [Description("Bizonylati tömb kód")]
        public string CounterCode { get; set; }
        [ColumnLabel("Leírás")]
        [Description("Bizonylati tömb leírás")]
        public string CounterDescription { get; set; }
        [ColumnLabel("Raktár ID")]
        [Description("Raktár ID")]
        public long WarehouseID { get; set; }
        [ColumnLabel("Előtag")]
        [Description("Előtag")]
        public string Prefix { get; set; }
        [ColumnLabel("Aktuális érték")]
        [Description("Számláló aktuális értéke")]
        public long CurrentNumber { get; set; }
        [ColumnLabel("Számláló mérete")]
        [Description("Számláló helyiértékének mérete")]
        public int NumbepartLength { get; set; }
        [ColumnLabel("Lezáró")]
        [Description("Lezáró karakter")]
        public string Suffix { get; set; }

        [ColumnLabel("Lezáratlan bizonylatok")]
        [Description("Lezáratlan bizonylatok")]
        public IList<CounterPoolItem> CounterPool { get; set; }


        //relációk
        [JsonIgnore]                    //ignorálni kell, mert körkörös hivatkozást eredményez
        [ForeignKey("WarehouseID")]
        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public Warehouse Warehouse { get; set; }


    }

    public class CounterPoolItem
    {
        public string CounterValue { get; set; }
        public long Ticks { get; set; }
    }
}
