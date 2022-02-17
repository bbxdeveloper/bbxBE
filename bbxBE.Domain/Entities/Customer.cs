using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string CustomerName { get; set; }

        public string CustomerBankAccountNumber { get; set; }

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
        public string VatCode { get; set; }
        public string ThirdStateTaxId { get; set; }
        public string CountryCode { get; set; }
        public string CountyCode { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string AdditionalAddressDetail { get; set; }
        public string Comment { get; set; }

    }
}
