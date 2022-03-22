using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace bbxBE.Domain.Entities
{
	[Description("Számlasor egyéb adat")]
	public class AdditionalInvoiceLineData : BaseEntity
	{
		[ColumnLabel("Számlasor ID")]
		[Description("Számlasor ID")]
		public long InvoiceLineID { get; set; }
		[ColumnLabel("Megnevezés")]
		[Description("MEgnevezés")]
		public string DataName { get; set; }
		[ColumnLabel("Leírás")]
		[Description("Leírás")]
		public string DataDescription { get; set; }
		[ColumnLabel("Érték")]
		[Description("Érték")]
		public string DataValue { get; set; }


		//Relációk
		[ForeignKey("InvoiceLineID")]
		[ColumnLabel("Számlasor")]
		[Description("Számlasor")]
		public InvoiceLine InvoiceLine { get; set; }
	}
}
