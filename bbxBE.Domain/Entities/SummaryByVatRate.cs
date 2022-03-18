using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace bbxBE.Domain.Entities
{
	public class SummaryByVatRate : BaseEntity
	{
		public long InvoiceID { get; set; }
		public long VatRateID { get; set; }
		public decimal VatRateNetAmount { get; set; }
		public decimal VatRateNetAmountHUF { get; set; }

		[ForeignKey("InvoiceID")]
		public Invoice Invoice { get; set; }

		[ForeignKey("VatRateID")]
		public VatRate VatRate { get; set; }


	}
}
