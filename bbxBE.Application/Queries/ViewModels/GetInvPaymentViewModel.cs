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
        public long InvoiceNumber { get; set; }

        [ColumnLabel("Fiz.hat")]
        [Description("Fizetési határidő dátuma")]
        public DateTime PaymentDate { get; set; }

        [ColumnLabel("Banki tranzakció")]
        [Description("Banki tranzakció azonosító")]
        public string BankTransaction { get; set; }

        [ColumnLabel("Dátum")]
        [Description("Banki tranzakció dátuma")]
        public DateTime InvPaymentDate { get; set; }



        [ColumnLabel("Összeg")]
        [Description("Banki tranzakció összege")]
        public decimal InvPaymentAmount { get; set; }
    }
}
