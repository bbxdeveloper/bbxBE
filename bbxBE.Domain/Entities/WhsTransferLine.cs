using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace bbxBE.Domain.Entities
{
    [Description("Raktárközi átadássor")]
    public class WhsTransferLine : BaseEntity
    {



        [ColumnLabel("Raktárközi átadás ID")]
        [Description("Raktárközi bizonylat átadás ID")]
        public long WhsTransferID { get; set; }

        [ColumnLabel("Sor száma")]
        [Description("Sor száma")]
        public short WhsTransferLineNumber { get; set; }

        [ColumnLabel("Termék ID")]
        [Description("Termék ID")]
        public long ProductID { get; set; }

        [ColumnLabel("Termékkód")]
        [Description("Termékkód")]
        public string ProductCode { get; set; }

        [ColumnLabel("Mennyiség")]
        [Description("Mennyiség")]
        public decimal Quantity { get; set; }
        [ColumnLabel("Me")]
        [Description("Mennyiségi egység")]
        public string UnitOfMeasure { get; set; }

        [ColumnLabel("Aktuális beszerzési egységár")]
        [Description("Aktuális beszerzési egységár")]

        public decimal CurrAvgCost { get; set; }


        [ForeignKey("ProductID")]
        [ColumnLabel("Termék")]
        [Description("Termék")]
        public Product Product { get; set; }
    }
}
