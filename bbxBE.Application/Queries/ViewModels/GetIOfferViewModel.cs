using AutoMapper.Configuration.Conventions;
using bbxBE.Common.Attributes;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace bbxBE.Application.Queries.ViewModels
{


	/// <summary>
	/// MapTo properties marks the names in the output Entity
	/// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
	/// In this case, <see cref="GetOfferViewModel"/> will be the value for the TDestination parameter.
	/// </summary>
	public class GetOfferViewModel
	{

		[Description("Árajánlat-sor")]
		public class OfferLine
		{
			[MapTo("ID")]
			public string ID { get; set; }
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

			[ColumnLabel("Áfaleíró-kód")]
			[Description("Áfaleíró-kód")]
			public string VatRateCode { get; set; }

			[ColumnLabel("Áfa%")]
			[Description("Az alkalmazott adó mértéke - Áfa tv. 169. § j)")]
			public decimal VatPercentage { get; set; }

			[ColumnLabel("Ár")]
			[Description("Ár")]
			public decimal UnitPrice { get; set; }

		}



		[MapTo("ID")]
		public long ID { get; set; }

		[ColumnLabel("Ajánlat száma")]
		[Description("Ajánlat száma")]
		public string OfferNumber { get; set; }

		[ColumnLabel("Kelt")]
		[Description("Kiállítás dátuma")]
		public DateTime OfferIssueDate { get; set; }

		[ColumnLabel("Érvényesség")]
		[Description("Érvényesség dátuma")]
		public DateTime OfferVaidityDate { get; set; }

		#region Customer
		[ColumnLabel("Ügyfél ID")]
		[Description("Ügyfél ID")]
		public long CustomerID { get; set; }

		[ColumnLabel("Ügyfélnév")]
		[Description("Ügyfélnév")]
		public string CustomerName { get; set; }

		[ColumnLabel("Bankszámlaszám")]
		[Description("Ügyfél bankszámlaszám")]
		public string CustomerBankAccountNumber { get; set; }


		[ColumnLabel("Adószám")]
		[Description("Ügyfél adószám")]
		public string CustomerTaxpayerNumber { get; set; }

		[ColumnLabel("Országkód")]
		[Description("Ügyfél országkód")]
		public string CustomerCountryCode { get; set; }

		[ColumnLabel("IRSZ")]
		[Description("Ügyfél irányítószám")]
		public string CustomerPostalCode { get; set; }

		[ColumnLabel("Város")]
		[Description("Ügyfél város")]
		public string CustomerCity { get; set; }

		[ColumnLabel("Cím")]
		[Description("Ügyfélcím")]
		public string CustomerAdditionalAddressDetail { get; set; }



		[ColumnLabel("Megjegyzés")]
		[Description("Ügyfél megjegyzés")]
		public string CustomerComment { get; set; }

		#endregion

		[ColumnLabel("Példány")]
		[Description("Nyomtatott példány száma")]
		public short Copies { get; set; }

		[ColumnLabel("Megjegyzés")]
		[Description("Megjegyzés")]
		public string Notice { get; set; }





	}
}
