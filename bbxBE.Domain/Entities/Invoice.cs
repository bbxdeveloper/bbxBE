using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
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
		private enInvoiceType invoiceType;
		public string InvoiceType
		{
			get { return Enum.GetName(typeof(enInvoiceType), invoiceType); }
			set
			{
				if (value != null)
					invoiceType = (enInvoiceType)Enum.Parse(typeof(enInvoiceType), value);
				else
					invoiceType = enInvoiceType.INV;

			}
		}
		public long WarehouseID { get; set; }

		public string InvoiceNumber { get; set; }
		public DateTime InvoiceIssueDate { get; set; }
		public bool CompletenessIndicator { get; set; }
		public long SupplierID { get; set; }
		public long CustomerID { get; set; }
	
		private InvoiceCategoryType invoiceCategory;
		public string InvoiceCategory
		{
			get { return Enum.GetName(typeof(InvoiceCategoryType), invoiceType); }
			set
			{
				if (value != null)
					invoiceCategory = (InvoiceCategoryType)Enum.Parse(typeof(InvoiceCategoryType), value);
				else
					invoiceCategory = InvoiceCategoryType.NORMAL;

			}
		}

		public DateTime InvoiceDeliveryDate { get; set; }
		public DateTime PaymentDate { get; set; }

	
		private PaymentMethodType paymentMethod;
		public string PaymentMethod
		{
			get { return Enum.GetName(typeof(PaymentMethodType), invoiceType); }
			set
			{
				if (value != null)
					paymentMethod = (PaymentMethodType)Enum.Parse(typeof(PaymentMethodType), value);
				else
					paymentMethod = PaymentMethodType.OTHER;

			}
		}

		private enCurrencyCodes currencyCode;
		public string CurrencyCode
		{
			get { return Enum.GetName(typeof(enCurrencyCodes), invoiceType); }
			set
			{
				if (value != null)
					currencyCode = (enCurrencyCodes)Enum.Parse(typeof(enCurrencyCodes), value);
				else
					currencyCode = enCurrencyCodes.HUF;

			}
		}

		public decimal ExchangeRate { get; set; }
		public bool UtilitySettlementIndicator { get; set; }
	
		private InvoiceAppearanceType invoiceAppearance;
		public string InvoiceAppearance
		{
			get { return Enum.GetName(typeof(InvoiceAppearanceType), invoiceType); }
			set
			{
				if (value != null)
					invoiceAppearance = (InvoiceAppearanceType)Enum.Parse(typeof(InvoiceAppearanceType), value);
				else
					invoiceAppearance = InvoiceAppearanceType.PAPER;

			}
		}
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
