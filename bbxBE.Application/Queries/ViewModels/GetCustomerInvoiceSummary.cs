using bbxBE.Common.Attributes;
using System.ComponentModel;

namespace bbxBE.Application.Queries.ViewModels
{


    /// <summary>
    /// MapToEntity properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetCustomerInvoiceSummary"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetCustomerInvoiceSummary
    {
        [ColumnLabel("B/K")]
        [Description("Bejővő/Kimenő")]
        public bool Incoming { get; set; }

        #region Customer
        [ColumnLabel("Ügyfél ID")]
        [Description("Ügyfél ID")]
        public long CustomerID { get; set; }

        [ColumnLabel("Ügyfélnév")]
        [Description("Ügyfélnév")]
        public string CustomerName { get; set; }

        [ColumnLabel("Cím")]
        [Description("Ügyfélcím")]
        public string CustomerFullAddress { get; set; }

        #endregion

        [ColumnLabel("Bizonylatok száma")]
        [Description("Bizonylatok száma")]
        public int InvoiceCount { get; set; }

        [ColumnLabel("Kedvezmény össz.")]
        [Description("A számlára adott teljes kedvezmény % értéke a számla pénznemében összesen")]
        public decimal InvoiceDiscountSum { get; set; }

        [ColumnLabel("Kedvezmény HUF össz.")]
        [Description("A számlára adott teljes kedvezmény % értéke fortintban összesen")]
        public decimal InvoiceDiscountHUFSum { get; set; }


        [ColumnLabel("Nettó összesen")]
        [Description("A számla nettó összege a számla pénznemében összesen")]
        public decimal InvoiceNetAmountSum { get; set; }

        [ColumnLabel("Nettó HUF összesen")]
        [Description("A számla nettó összege forintban összesen")]
        public decimal InvoiceNetAmountHUFSum { get; set; }

        [ColumnLabel("Áfa összesen")]
        [Description("A számla áfa összege a számla pénznemében összesen")]
        public decimal InvoiceVatAmountSum { get; set; }

        [ColumnLabel("Áfa HUF összesen")]
        [Description("A számla áfa összege forintban összesen")]
        public decimal InvoiceVatAmountHUFSum { get; set; }

        [ColumnLabel("Bruttó összesen")]
        [Description("A számla végösszege a számla pénznemében összesen")]
        public decimal InvoiceGrossAmountSum { get; set; }

        [ColumnLabel("Bruttó HUF összesen")]
        [Description("A számla végösszege forintban összesen")]
        public decimal InvoiceGrossAmountHUFSum { get; set; }

    }
}
