using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace bbxBE.Domain.Entities
{
	public class AdditionalInvoiceData : BaseEntity
	{
	
		public long InvoiceID { get; set; }
		public string DataName { get; set; }
		public string DataDescription { get; set; }
		public string DataValue { get; set; }

		[ForeignKey("InvoiceID")]
		public Invoice Invoice { get; set; }

	}
}
