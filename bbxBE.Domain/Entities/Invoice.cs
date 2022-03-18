using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace bbxBE.Domain.Entities
{
	public class Invoice : BaseEntity
	{
		public bool Incoming { get; set; }
		private enInvoiceType IivoiceType { get; set; }

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
		public long WarehouseID { get; set; }

		public string InvoiceNumber { get; set; }
		public DateTime InvoiceIssueDate { get; set; }
		public bool CompletenessIndicator { get; set; }
		public long SupplierID { get; set; }
		public long CustomerID { get; set; }
		public string InvoiceCategory { get; set; }
		public DateTime InvoiceDeliveryDate { get; set; }
		public DateTime PaymentDate { get; set; }
		public string CurrencyCode { get; set; }
		public decimal ExchangeRate { get; set; }
		public bool UtilitySettlementIndicator { get; set; }
		public string InvoiceAppearance { get; set; }
		public long OriginalInvoiceID { get; set; }
		public string OriginalInvoiceNumber { get; set; }
		public bool ModifyWithoutMaster { get; set; }
		public short ModificationIndex { get; set; }
		public string OrderNumber { get; set; }
		public decimal InvoiceNetAmount { get; set; }
		public decimal InvoiceNetAmountHUF { get; set; }
		public decimal InvoiceVatAmount { get; set; }
		public decimal InvoiceVatAmountHUF { get; set; }
		public decimal InvoiceGrossAmount { get; set; }
		public decimal invoiceGrossAmountHUF { get; set; }

		[ForeignKey("WarehouseID")]
		public Warehouse Warehouse { get; set; }

		[ForeignKey("SupplierID")]
		public Customer Supplier { get; set; }

		[ForeignKey("CustomerID")]
		public Customer Customer { get; set; }

		[ForeignKey("OriginalInvoiceID")]
		public Invoice OriginalInvoice { get; set; }

		[ForeignKey("InvoiceID")]
		public ICollection<AdditionalInvoiceData> AdditionalInvoiceData { get; set; }

		[ForeignKey("InvoiceID")]
		public ICollection<SummaryByVatRate> SummaryByVatRates { get; set; }
	}
}
