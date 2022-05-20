using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace bbxBE.Domain.Entities
{
	[Description("Árajánlat-sor")]
	public class OfferLine : BaseEntity
	{
		[ColumnLabel("Árajánlat ID")]
		[Description("Árajánlat ID")]
		public long OfferID { get; set; }

		[ColumnLabel("#")]
		[Description("Sor száma")]
		public short LineNumber { get; set; }

		[ColumnLabel("Termék ID")]
		[Description("Termék ID")]
		public long? ProductID { get; set; }

		[ColumnLabel("Termékkód")]
		[Description("Termékkód")]
		public string ProductCode { get; set; }

		[ColumnLabel("Megnevezés")]
		[Description("A termék vagy szolgáltatás megnevezése")]
		public string LineDescription { get; set; }

		[ColumnLabel("Áfa ID")]
		[Description("Áfa ID")]
		public long VatRateID { get; set; }

		[ColumnLabel("Áfa%")]
		[Description("Az alkalmazott adó mértéke - Áfa tv. 169. § j)")]
		public decimal VatPercentage { get; set; }

		[ColumnLabel("Ár")]
		[Description("Ár")]
		public decimal UnitPrice { get; set; }

		[ColumnLabel("Ár forintban")]
		[Description("Ár forintban")]
		public decimal UnitPriceHUF { get; set; }

		[ColumnLabel("Áfa értéke")]
		[Description("Áfa értéke")]
		public decimal UnitVat { get; set; }

		[ColumnLabel("Áfa értéke forintban")]
		[Description("Áfa értéke forintban")]
		public decimal UnitVatHUF { get; set; }

		[ColumnLabel("Bruttó ár")]
		[Description("Bruttó ár")]
		public decimal UnitGross { get; set; }

		[ColumnLabel("Bruttó ár forintban")]
		[Description("Bruttó ár forintban")]
		public decimal UnitGrossHUF { get; set; }

		//Relációk
		[JsonIgnore]					//ignorálni kell, mert körkörös hivatkozást eredményez
		[ForeignKey("OfferID")]
		[ColumnLabel("Árajánlat")]
		[Description("Árajánlat")]
		public Offer Offer { get; set; }

		[ForeignKey("ProductID")]
		[ColumnLabel("Termék")]
		[Description("Termék")]
		public Product Product { get; set; }

		[ForeignKey("VatRateID")]
		[ColumnLabel("Áfakulcs")]
		[Description("Áfakulcs")]
		public VatRate VatRate { get; set; }

	}
}
