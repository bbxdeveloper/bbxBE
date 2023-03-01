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
	/// MapToEntity properties marks the names in the output Entity
	/// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
	/// In this case, <see cref="GetAggregateInvoiceViewModel"/> will be the value for the TDestination parameter.
	/// </summary>
	public class GetAggregateInvoiceViewModel
	{

        [Description("Áfaösszesítő")]
		public class SummaryByVatRate 
		{
			[ColumnLabel("Áfa ID")]
			[Description("Áfa ID")]
			public long VatRateID { get; set; }

			[ColumnLabel("Áfaleíró-kód")]
			[Description("Áfaleíró-kód")]
			public string VatRateCode { get; set; }

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

		}


		[MapToEntity("ID")]
		public long ID { get; set; }

		[ColumnLabel("Raktár ID")]
		[Description("Raktár ID")]
		public long WarehouseID { get; set; }

		[ColumnLabel("Raktár")]
		[Description("Raktár")]
		public string Warehouse { get; set; }


		[ColumnLabel("Számlaszám")]
		[Description("Számla sorszáma")]
		public string InvoiceNumber { get; set; }

		[ColumnLabel("Kelt")]
		[Description("Kiállítás dátuma")]
		public DateTime InvoiceIssueDate { get; set; }

		[ColumnLabel("Teljesítés")]
		[Description("Teljesítés dátuma")]
		public DateTime InvoiceDeliveryDate { get; set; }

		[ColumnLabel("Fiz.hat")]
		[Description("Fizetési határidő dátuma")]
		public DateTime PaymentDate { get; set; }

		#region Supplier
		[ColumnLabel("Szállító ID")]
		[Description("Szállító ID")]
		public long SupplierID { get; set; }

		[ColumnLabel("Szállítónév")]
		[Description("Szállítónév")]
		public string SupplierName { get; set; }

		[ColumnLabel("Bankszámlaszám")]
		[Description("Szállító bankszámlaszám")]
		public string SupplierBankAccountNumber { get; set; }


		[ColumnLabel("Adószám")]
		[Description("Szállító adószám")]
		public string SupplierTaxpayerNumber { get; set; }

		[ColumnLabel("Országkód")]
		[Description("Szállító országkód")]
		public string SupplierCountryCode { get; set; }

		[ColumnLabel("IRSZ")]
		[Description("Szállító irányítószám")]
		public string SupplierPostalCode { get; set; }

		[ColumnLabel("Város")]
		[Description("Szállító város")]
		public string SupplierCity { get; set; }

		[ColumnLabel("Cím")]
		[Description("Szállítócím")]
		public string SupplierAdditionalAddressDetail { get; set; }

		[ColumnLabel("Külföldi adószám")]
		[Description("Külföldi adószám")]
		public string SupplierThirdStateTaxId { get; set; }

		[ColumnLabel("Megjegyzés")]
		[Description("Szállító megjegyzés")]
		public string SupplierComment { get; set; }

		#endregion

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

		[ColumnLabel("Külföldi adószám")]
		[Description("Külföldi adószám")]
		public string CustomerThirdStateTaxId { get; set; }


		[ColumnLabel("Megjegyzés")]
		[Description("Ügyfél megjegyzés")]
		[MapToEntity("customerComment")]
		public string CustomerComment { get; set; }

		#endregion

		#region PaymentMethod

		[DataMember]
		[ColumnLabel("Fiz.mód")]
		[Description("Fizetési mód")]
		public string PaymentMethod { get; set; }

		[ColumnLabel("Fiz.mód")]
		[Description("Fizetési mód megnevezés")]
		[DataMember]
		[NotDBField]
		public string PaymentMethodX { get; set; }
		#endregion

		[ColumnLabel("Eredeti.biz")]
		[Description("Bevételhez eredeti bizonylata")]
		[MapToEntity("customerInvoiceNumber")]
		public string CustomerInvoiceNumber { get; set; }

		[ColumnLabel("Megjegyzés")]
		[Description("Megjegyzés")]
		public string Notice { get; set; }  //AdditionalInvoiceData-ban tároljuk!

		[ColumnLabel("Példány")]
		[Description("Nyomtatott példány száma")]
		public short Copies { get; set; }

        [ColumnLabel("Kedvezmény%")]
        [Description("A számlára adott teljes kedvezmény %")]
        public decimal InvoiceDiscountPercent { get; set; }
        [ColumnLabel("Kedvezmény")]
        [Description("A számlára adott teljes kedvezmény % értéke a számla pénznemében")]
        public decimal InvoiceDiscount { get; set; }

        [ColumnLabel("Kedvezmény HUF")]
        [Description("A számlára adott teljes kedvezmény % értéke fortintban")]
        public decimal InvoiceDiscountHUF { get; set; }


        [ColumnLabel("Nettó")]
		[Description("A számla nettó összege a számla pénznemében")]
		public decimal InvoiceNetAmount { get; set; }

        [ColumnLabel("Nettó HUF")]
        [Description("A számla nettó összege forintban")]
        public decimal InvoiceNetAmountHUF { get; set; }
        
		[ColumnLabel("Áfa")]
		[Description("A számla áfa összege a számla pénznemében")]
		public decimal InvoiceVatAmount { get; set; }

        [ColumnLabel("Áfa HUF")]
        [Description("A számla áfa összege forintban")]
        public decimal InvoiceVatAmountHUF { get; set; }

        [ColumnLabel("Bruttó")]
		[Description("A számla végösszege a számla pénznemében")]
		public decimal InvoiceGrossAmount { get; set; }

        [ColumnLabel("Bruttó HUF")]
        [Description("A számla végösszege forintban")]
        public decimal InvoiceGrossAmountHUF { get; set; }

        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long UserID { get; set; }

        [ColumnLabel("Felhasználó")]
        [Description("Felhasználó")]
        [MapToEntity("UserName")]
        public string UserName { get; set; }

        [ColumnLabel("Munkaszám")]
        [Description("Munkaszám")]
        public string WorkNumber { get; set; }

        [ColumnLabel("Ár felülvizsgálat?")]
        [Description("Ár felülvizsgálat?")]
        public bool PriceReview { get; set; } = false;


		[ColumnLabel("Áfaösszesítők")]
		[Description("Áfaösszesítők")]
		[MapToEntity("summaryByVatRates")]
		public List<GetAggregateInvoiceViewModel.SummaryByVatRate> SummaryByVatRates { get; set; } = new List<GetAggregateInvoiceViewModel.SummaryByVatRate>();

		[ColumnLabel("Szállítólvelek")]
		[Description("Szállítólvelek")]
		[MapToEntity("deliveryNotes")]
		public List<GetAggregateInvoiceDeliveryNoteViewModel> DeliveryNotes { get; set; } = new List<GetAggregateInvoiceDeliveryNoteViewModel>();

		[ColumnLabel("Szállítólvelek száma")]
		[Description("Szállítólvelek száma")]
		[MapToEntity("deliveryNotesCount")]
		public int DeliveryNoteCount { get; set; }
	}
}
