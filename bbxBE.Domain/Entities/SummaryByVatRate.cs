using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace bbxBE.Domain.Entities
{
	[Description("Számla áfánkénti összesítő")]
	public class SummaryByVatRate : BaseEntity
	{
		[ColumnLabel("Számla ID")]
		[Description("Számla ID")]
		public long InvoiceID { get; set; }
		[ColumnLabel("Áfa ID")]
		[Description("Áfa ID")]
		public long VatRateID { get; set; }

		[ColumnLabel("Áfaalap")]
		[Description("Áfaalap számla pénznemében")]
		public decimal VatRateNetAmount { get; set; }
		[ColumnLabel("Áfaalap HUF")]
		[Description("Áfaalap forintban")]
		public decimal VatRateNetAmountHUF { get; set; }

		[ColumnLabel("Áfa értéke")]
		[Description("Áfa értéke számla pénznemében")]
		public decimal VatRateVatAmount { get; set; }
		[ColumnLabel("Áfa értéke HUF")]
		[Description("Áfa értéke forintban")]
		public decimal VatRateVatAmountHUF { get; set; }

		[ColumnLabel("Bruttó érték")]
		[Description("Bruttó érték számla pénznemében")]
		public decimal VatRateGrossAmount { get; set; }
		[ColumnLabel("Bruttó érték HUF")]
		[Description("Bruttó érték forintban")]
		public decimal VatRateGrossAmountHUF { get; set; }


		//Relációk
		[ForeignKey("InvoiceID")]
		[ColumnLabel("Számla")]
		[Description("Számla")]
		public Invoice Invoice { get; set; }

		[ForeignKey("VatRateID")]
		[ColumnLabel("Áfakulcs")]
		[Description("Áfakulcs")]
		public VatRate VatRate { get; set; }


	}
}
