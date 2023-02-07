using AutoMapper.Configuration.Conventions;
using bbxBE.Common.Attributes;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace bbxBE.Application.Queries.ViewModels
{


	/// <summary>
	/// MapToEntity properties marks the names in the output Entity
	/// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
	/// In this case, <see cref="GetInvoiceViewModel"/> will be the value for the TDestination parameter.
	/// </summary>
	public class GetPendigDeliveryNotesModel
    {

        [ColumnLabel("Szállítólevél sor ID")]
        [Description("Szállítólevél sor ID")]
        public string InvoiceLineID { get; set; }

        [ColumnLabel("Raktár ID")]
		[Description("Raktár ID")]
		public long WarehouseID { get; set; }

		[ColumnLabel("Ügyfél ID")]
		[Description("Ügyfél ID")]
		public long CustomerID { get; set; }

		[ColumnLabel("Ügyfélnév")]
		[Description("Ügyfélnév")]
		public string Customer { get; set; }

        [ColumnLabel("Szállítólevél szám")]
        [Description("Szállítólevél szám")]
        public string InvoiceNumber { get; set; }

        [ColumnLabel("Teljesítés")]
        [Description("Teljesítés dátuma")]
        public DateTime InvoiceDeliveryDate { get; set; }


        [ColumnLabel("#")]
        [Description("Sor száma")]
        public short LineNumber { get; set; }

        [ColumnLabel("")]
        [Description("Szállítólevél sor ID")]
        public short OrigDeliveryNoteInvoiceLineID { get; set; }

        [ColumnLabel("Termékkód")]
        [Description("Termékkód")]
        public string ProductCode { get; set; }

        [ColumnLabel("Áfa ID")]
        [Description("Áfa ID")]
        public long VatRateID { get; set; }

        [ColumnLabel("Áfaleíró-kód")]
        [Description("Áfaleíró-kód")]
        public string VatRateCode { get; set; }

        [ColumnLabel("Áfa%")]
        [Description("Az alkalmazott adó mértéke - Áfa tv. 169. § j)")]
        public decimal VatPercentage { get; set; }

        [ColumnLabel("Megnevezés")]
        [Description("A termék vagy szolgáltatás megnevezése")]
        public string LineDescription { get; set; }

        [ColumnLabel("Mennyiség")]
        [Description("Mennyiség")]
        public decimal Quantity { get; set; }
        #region UnitOfMeasure

        [DataMember]
        [ColumnLabel("Me.e")]
        [Description("Mennyiségi egység")]
        public string UnitOfMeasure { get; set; }

        [ColumnLabel("Me.e név")]
        [Description("Mennyiségi egység megnevezés")]
        [DataMember]
        [NotDBField]
        public string UnitOfMeasureX { get; set; }
        #endregion

        [ColumnLabel("Ár")]
        [Description("Ár")]
        public decimal UnitPrice { get; set; }

        [ColumnLabel("Nettó érték")]
        [Description("Ár a számla pénznemében")]
        public decimal LineNetAmount { get; set; }

    }
}
