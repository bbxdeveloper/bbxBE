using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace bbxBE.Domain.Entities
{
	public class InvoiceLine : BaseEntity
	{
		public long InvoiceID { get; set; }
		public long ProductID { get; set; }
		public long VatRateID { get; set; }
		public short LineNumber { get; set; }
		public bool LineExpressionIndicator { get; set; }
		public string LineNatureIndicator { get; set; }
		public string LineDescription { get; set; }
		public decimal Quantity { get; set; }
		public string UnitOfMeasure { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal UnitPriceHUF { get; set; }
		public decimal LineNetAmount { get; set; }
		public decimal LineNetAmountHUF { get; set; }
		public decimal lineVatAmount { get; set; }
		public decimal lineVatAmountHUF { get; set; }
		public decimal lineGrossAmountNormal { get; set; }
		public decimal lineGrossAmountNormalHUF { get; set; }
		public short LineNumberReference { get; set; }
		public string LineOperation { get; set; }
		public decimal LineExchangeRate { get; set; }
		public DateTime LineDeliveryDate { get; set; }
		public string DeliveryNote { get; set; }
		public string TakeoverReason { get; set; }
		public decimal TakeoverAmount { get; set; }
		public string TakeoverproductCodeCategory { get; set; }
		public string TakeoverproductCodeValue { get; set; }
		public decimal ProductFeeQuantity { get; set; }
		public string ProductFeeMeasuringUnit { get; set; }
		public decimal ProductFeeRate { get; set; }
		public decimal ProductFeeAmount { get; set; }


		[ForeignKey("InvoiceID")]
		public Invoice Invoice { get; set; }

		[ForeignKey("ProductID")]
		public Product Product { get; set; }

		[ForeignKey("VatRateID")]
		public VatRate VatRate { get; set; }

	}
}
