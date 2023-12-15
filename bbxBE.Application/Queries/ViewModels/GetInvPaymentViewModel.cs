using bbxBE.Common.Attributes;
using System;
using System.ComponentModel;

namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapToEntity properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetWhsTransferViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetInvPaymentViewModel
    {
        [ColumnLabel("Számla ID")]
        [Description("Számla ID")]
        public long InvoiceID { get; set; }

        [ColumnLabel("Számlaszám")]
        [Description("Számla száma")]
        public string InvoiceNumber { get; set; }

        [ColumnLabel("Fiz.hat")]
        [Description("Fizetési határidő dátuma")]
        public DateTime PaymentDate { get; set; }

        [ColumnLabel("Partner ID")]
        [Description("Partner ID")]
        public long CustomerID { get; set; }

        [ColumnLabel("Partnernév")]
        [Description("Partnernév")]
        public string CustomerName { get; set; }

        [ColumnLabel("Banki tranzakció")]
        [Description("Banki tranzakció azonosító")]
        public string BankTransaction { get; set; }

        [ColumnLabel("Dátum")]
        [Description("Banki tranzakció dátuma")]
        public DateTime InvPaymentDate { get; set; }

        [ColumnLabel("Pénznem kód")]
        [Description("Pénznem kód")]
        public string CurrencyCode { get; set; }

        [ColumnLabel("Pénznem")]
        [Description("Pénznem")]
        public string CurrencyCodeX { get; set; }

        [ColumnLabel("Árfolyam")]
        [Description("Árfolyam")]
        public decimal ExchangeRate { get; set; }

        [ColumnLabel("Összeg")]
        [Description("Banki tranzakció összege")]
        public decimal InvPaymentAmount { get; set; }

        [ColumnLabel("Összeg HUF")]
        [Description("Banki tranzakció összege forintban")]
        public decimal InvPaymentAmountHUF { get; set; }

        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long UserID { get; set; }

        [ColumnLabel("Felhasználó")]
        [Description("Felhasználó")]
        [MapToEntity("UserName")]
        public string UserName { get; set; }


    }
}
