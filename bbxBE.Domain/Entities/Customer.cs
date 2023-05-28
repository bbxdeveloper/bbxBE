using bbxBE.Common.Attributes;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Common;
using System;
using System.ComponentModel;

namespace bbxBE.Domain.Entities
{
    [Description("Ügyfelek,partnerek,saját adat")]
    public class Customer : BaseEntity
    {
        [ColumnLabel("Név")]
        [Description("Ügyfélnév")]
        public string CustomerName { get; set; }

        [ColumnLabel("Bankszámlaszám")]
        [Description("Bankszámlaszám")]
        public string CustomerBankAccountNumber { get; set; }

        [ColumnLabel("Ügyféltípus")]
        [Description("Ügyféltípus: Mo., külföldi áfaalany, magánszemély")]
        private CustomerVatStatusType customerVatStatus;
        public string CustomerVatStatus
        {
            get { return Enum.GetName(typeof(CustomerVatStatusType), customerVatStatus); }
            set
            {
                if (value != null)
                    customerVatStatus = (CustomerVatStatusType)Enum.Parse(typeof(CustomerVatStatusType), value);
                else
                    customerVatStatus = CustomerVatStatusType.DOMESTIC;

            }
        }

        public string TaxpayerId { get; set; }
        [ColumnLabel("Áfakód")]
        [Description("Áfakód")]
        public string VatCode { get; set; }
        [ColumnLabel("Megyekód")]
        [Description("Megyekód, az adószám harmadik része")]
        public string CountyCode { get; set; }
        [ColumnLabel("Külföldi adószám")]
        [Description("Külföldi adószám")]
        public string ThirdStateTaxId { get; set; }
        [ColumnLabel("Országkód")]
        [Description("Országkód")]
        public string CountryCode { get; set; }
        [ColumnLabel("Régiókód")]
        [Description("Régiókód")]
        public string Region { get; set; }
        [ColumnLabel("IRSZ")]
        [Description("Irányítószám")]
        public string PostalCode { get; set; }
        [ColumnLabel("Város")]
        [Description("Város")]
        public string City { get; set; }
        [ColumnLabel("Cím")]
        [Description("Cím")]
        public string AdditionalAddressDetail { get; set; }
        [ColumnLabel("Email")]
        [Description("Email")]
        public string Email { get; set; }
        [ColumnLabel("Megjegyzés")]
        [Description("Megjegyzés")]
        public string Comment { get; set; }

        [ColumnLabel("Eladási ártípus")]
        [Description("Eladási ártípus")]
        private enUnitPriceType unitPriceType;
        public string UnitPriceType
        {
            get { return Enum.GetName(typeof(enUnitPriceType), unitPriceType); }
            set
            {
                if (value != null)
                    unitPriceType = (enUnitPriceType)Enum.Parse(typeof(enUnitPriceType), value);
                else
                    unitPriceType = enUnitPriceType.LIST;
            }
        }

        [ColumnLabel("Fizetési határidő")]
        [Description("Fizetési határidő (napban)")]
        public short PaymentDays { get; set; }

        [ColumnLabel("Figyelmeztetés limit")]
        [Description("Figyelmeztetés limit")]
        public decimal? WarningLimit { get; set; }

        [ColumnLabel("Maximális limit")]
        [Description("Maximális limit")]
        public decimal? MaxLimit { get; set; }

        [ColumnLabel("Alap.fiz.mód")]
        [Description("Alapértelmezett fizetési mód")]
        private PaymentMethodType defPaymentMethod;
        public string DefPaymentMethod
        {
            get { return Enum.GetName(typeof(PaymentMethodType), unitPriceType); }
            set
            {
                if (value != null)
                    defPaymentMethod = (PaymentMethodType)Enum.Parse(typeof(PaymentMethodType), value);
                else
                    defPaymentMethod = PaymentMethodType.CASH;
            }
        }


        [ColumnLabel("Saját adat?")]
        [Description("Saját adat? (csak egy ilyen rekord lehet)")]
        public bool IsOwnData { get; set; }
    }
}
