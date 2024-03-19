using bbxBE.Common.Attributes;
using bbxBE.Common.Enums;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace bbxBE.Domain.Entities
{
    [Description("Megrendelés fej")]
    public class POrder : BaseEntity
    {
        [ColumnLabel("Raktár ID")]
        [Description("Raktár ID")]
        public long WarehouseID { get; set; }

        [ColumnLabel("Szállító ID")]
        [Description("Szállító ID")]
        public long SupplierID { get; set; }

        [ColumnLabel("Ügyfél ID")]
        [Description("Ügyfél ID")]
        public long? CustomerID { get; set; }           //A blokk miatt nullable!

        [ColumnLabel("Ajánlat száma")]
        [Description("Ajánlat száma")]
        public long SupplierID { get; set; }

        [ColumnLabel("Kelt")]
        [Description("Kiállítás dátuma")]
        public DateTime OfferIssueDate { get; set; }

        [ColumnLabel("Érvényesség")]
        [Description("Érvényesség dátuma")]
        public DateTime OfferVaidityDate { get; set; }

        [ColumnLabel("Ügyfél ID")]
        [Description("Ügyfél ID")]
        public long CustomerID { get; set; }


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

        [ColumnLabel("Bruttó árak?")]
        [Description("Bruttó árak megjelenítése?")]
        public bool IsBrutto { get; set; }


        //relációk


        [ForeignKey("CustomerID")]
        [ColumnLabel("Ügyfél")]
        [Description("Ügyfél")]
        public virtual Customer Customer { get; set; }

        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long? UserID { get; set; } = 0;

        [ForeignKey("UserID")]
        [ColumnLabel("Felhasználó")]
        [Description("Felhasználó")]
        public virtual Users User { get; set; }

        [ColumnLabel("Árajánlat-sorok")]
        [Description("Árajánlat-sorok")]
        //		[InverseProperty("ID")]
        public virtual IList<OfferLine> OfferLines { get; set; }

    }
}
