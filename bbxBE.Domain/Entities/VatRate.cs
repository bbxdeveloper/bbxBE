using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Domain.Entities
{
	public class VatRate : BaseEntity
	{
		public string VatCode { get; set; }
		public decimal VatPercentage { get; set; }
		public decimal VatContent { get; set; }
		public string VatExemptionCase { get; set; }
		public string VatExemptionReason { get; set; }
		public string VatOutOfScopeCase { get; set; }
		public string VatOutOfScopeReason { get; set; }
		public bool VatDomesticReverseCharge { get; set; }
		public string MarginSchemeIndicator { get; set; }
		public decimal vatAmountMismatchVatRate { get; set; }
		public string vatAmountMismatchCase { get; set; }
		public bool NoVatCharge { get; set; }

		}
}
