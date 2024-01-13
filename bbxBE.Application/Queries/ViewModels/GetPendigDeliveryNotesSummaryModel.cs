using bbxBE.Common.Attributes;
using System.ComponentModel;

namespace bbxBE.Application.Queries.ViewModels
{


    /// <summary>
    /// MapToEntity properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetInvoiceViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetPendigDeliveryNotesSummaryModel
    {
        [ColumnLabel("Raktár ID")]
        [Description("Raktár ID")]
        public long WarehouseID { get; set; }

        [ColumnLabel("Ügyfél ID")]
        [Description("Ügyfél ID")]
        public long CustomerID { get; set; }

        [ColumnLabel("Ügyfélnév")]
        [Description("Ügyfélnév")]
        public string Customer { get; set; }

        [ColumnLabel("Cím")]
        [Description("Cím")]
        public string FullAddress { get; set; }

        [ColumnLabel("Nettó")]
        [Description("A kiszámlázatan összérték a szállítólevél pénznemében")]
        public decimal SumNetAmount { get; set; }

        [ColumnLabel("Kedvezménnyel csökkentett nettó")]
        [Description("A kiszámlázatan összérték a szállítólevél pénznemében")]
        public decimal SumNetAmountDiscounted { get; set; }

        [ColumnLabel("Ár felülvizsgálat?")]
        [Description("Ár felülvizsgálat?")]
        public bool PriceReview { get; set; } = false;

        [ColumnLabel("Pénznem kód")]
        [Description("Pénznem kód")]
        public string CurrencyCode { get; set; }

        [ColumnLabel("Pénznem")]
        [Description("Pénznem")]
        public string CurrencyCodeX { get; set; }

    }
}
