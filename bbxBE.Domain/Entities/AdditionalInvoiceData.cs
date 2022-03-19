using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace bbxBE.Domain.Entities
{
	[Description("Számla egyéb adat")]
	public class AdditionalInvoiceData : BaseEntity
	{

		[ColumnLabel("Számla ID")]
		[Description("Számla ID")]
		public long InvoiceID { get; set; }
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
		[ForeignKey("InvoiceID")]
		[ColumnLabel("Számla")]
		[Description("Számla")]
		public Invoice Invoice { get; set; }
	}
}
