using bbxBE.Common.Attributes;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace bbxBE.Domain.Entities
{
  	[Description("Árajánlat fej")]
	public class Offer : BaseEntity
	{

		[ColumnLabel("Ajánlat száma")]
		[Description("Ajánlat száma")]
		public string OfferNumber { get; set; }

		[ColumnLabel("Kelt")]
		[Description("Kiállítás dátuma")]
		public DateTime OfferIssueDate { get; set; }

		[ColumnLabel("Érvényesség")]
		[Description("Érvényesség dátuma")]
		public DateTime OfferVaidityDate { get; set; }

		[ColumnLabel("Ügyfél ID")]
		[Description("Ügyfél ID")]
		public long CustomerID { get; set; }


		[ColumnLabel("Példány")]
		[Description("Nyomtatott példány száma")]
		public short Copies { get; set; }

		[ColumnLabel("Megjegyzés")]
		[Description("Megjegyzés")]
		public string Notice { get; set; }

		[ColumnLabel("Verzió")]
		[Description("Verzió")]
		public short OfferVersion { get; set; }

		[ColumnLabel("Legutolsó verzió?")]
		[Description("Legutolsó verzió?")]
		public bool LatestVersion { get; set; }


		[ColumnLabel("Pénznem")]
		[Description("Pénznem")]
		private enCurrencyCodes currencyCode;
		public string CurrencyCode
		{
			get { return Enum.GetName(typeof(enCurrencyCodes), currencyCode); }
			set
			{
				if (value != null)
					currencyCode = (enCurrencyCodes)Enum.Parse(typeof(enCurrencyCodes), value);
				else
					currencyCode = enCurrencyCodes.HUF;

			}
		}
		[ColumnLabel("Árfolyam")]
		[Description("Árfolyam")]
		public decimal ExchangeRate { get; set; }


		//relációk


		[ForeignKey("CustomerID")]
		[ColumnLabel("Ügyfél")]
		[Description("Ügyfél")]
		[Required] 
		public virtual Customer Customer { get; set; }

		[ColumnLabel("Árajánlat-sorok")]
		[Description("Árajánlat-sorok")]
		[Required]
		public virtual ICollection<OfferLine> OfferLines { get; set; }

	}
}
