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
	/// In this case, <see cref="GetInvoiceViewModel"/> will be the value for the TDestination parameter.
	/// </summary>
	public class GetInvoiceViewModel
    {


		[Description("Számlasor")]
		public class InvoiceLine
		{

			[MapTo("ID")]
			public string ID { get; set; }

			[ColumnLabel("#")]
			[Description("Sor száma")]
			public short LineNumber { get; set; }

			[ColumnLabel("Termékkód")]
			[Description("Termékkód")]
			public string ProductCode { get; set; }

			[ColumnLabel("Áfakód")]
			[Description("Áfakód")]
			public string VatRateCode { get; set; }

			[ColumnLabel("Mennyiség")]
			[Description("Mennyiség")]
			public decimal Quantity { get; set; }

			#region UnitOfMeasure
			[IgnoreDataMember]
			[NotDBField]
			[NotModelField]
			private enUnitOfMeasure _UnitOfMeasure { get; set; }

			[DataMember]
			[ColumnLabel("Me.e")]
			[Description("Mennyiségi egység")]
			public string UnitOfMeasure
			{
				get { return Enum.GetName(typeof(enUnitOfMeasure), _UnitOfMeasure); }
				set
				{
					if (value != null)
						_UnitOfMeasure = (enUnitOfMeasure)Enum.Parse(typeof(enUnitOfMeasure), value);
					else
						_UnitOfMeasure = enUnitOfMeasure.PIECE;
				}
			}

			[ColumnLabel("Me.e név")]
			[Description("Mennyiségi egység megnevezés")]
			[DataMember]
			[NotDBField]
			public string UnitOfMeasureX { get { return Common.Utils.GetEnumDescription(_UnitOfMeasure); } }
			#endregion

			[ColumnLabel("Ár")]
			[Description("Ár")]
			public decimal Price { get; set; }

			[ColumnLabel("Nettó érték")]
			[Description("Ár a számla pénznemében")]
			public decimal LineNetAmount { get; set; }

			[ColumnLabel("Áfa érték")]
			[Description("Áfa a számla pénznemében")]
			public decimal lineVatAmount { get; set; }

		}

		[MapTo("ID")]
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


		[ColumnLabel("Megjegyzés")]
		[Description("Ügyfél megjegyzés")]
		public string CustomerComment { get; set; }

        #endregion

        #region PaymentMethod
        [IgnoreDataMember]
		[NotDBField]
		[NotModelField]
		private PaymentMethodType _PaymentMethod { get; set; }

		[DataMember]
		[ColumnLabel("Fiz.mód")]
		[Description("Fizetési mód")]
		public string PaymentMethod
		{
			get { return Enum.GetName(typeof(PaymentMethodType), _PaymentMethod); }
			set
			{
				if (value != null)
					_PaymentMethod = (PaymentMethodType)Enum.Parse(typeof(PaymentMethodType), value);
				else
					_PaymentMethod = PaymentMethodType.CASH;
			}
		}

		[ColumnLabel("Fiz.mód")]
		[Description("Fizetési mód megnevezés")]
		[DataMember]
		[NotDBField]
		public string PaymentMethodX { get { return Common.Utils.GetEnumDescription(_PaymentMethod); } }
		#endregion


		[ColumnLabel("Megjegyzés")]
		[Description("Megjegyzés")]
		public string Notice { get; set; }  //AdditionalInvoiceData-ban tároljuk!

		[ColumnLabel("Nettó")]
		[Description("A számla nettó összege a számla pénznemében")]
		public decimal InvoiceNetAmount { get; set; }

		[ColumnLabel("Áfa")]
		[Description("A számla áfa összege a számla pénznemében")]
		public decimal InvoiceVatAmount { get; set; }

		[ColumnLabel("Számlasorok")]
		[Description("Számlasorok")]
		public List<GetInvoiceViewModel.InvoiceLine> InvoiceLines { get; set; } = new List<GetInvoiceViewModel.InvoiceLine>();




    }
}
