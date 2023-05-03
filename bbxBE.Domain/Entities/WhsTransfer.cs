using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace bbxBE.Domain.Entities
{
    [Description("Raktárközi átadás")]
    public class WhsTransfer : BaseEntity
    {



        [ColumnLabel("Raktárközi átadás száma")]
        [Description("Raktárközi bizonylat átadás száma")]
        public string WhsTransferNumber { get; set; }

        [ColumnLabel("Kiadás raktár")]
        [Description("Kiadás raktár")]
        public long FromWarehouseID { get; set; }

        [ColumnLabel("Bevétel raktár")]
        [Description("Bevétel raktár")]
        public long ToWarehouseID { get; set; }

        [ColumnLabel("Dátum")]
        [Description("Dátum")]
        public long TransferDate { get; set; }

        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long? UserID { get; set; } = 0;


        [ForeignKey("FromWarehouseID")]
        [ColumnLabel("Kiadás raktár")]
        [Description("Kiadás raktár")]
        public Warehouse FromWarehouse { get; set; }

        [ForeignKey("ToWarehouseID")]
        [ColumnLabel("Bevétel raktár")]
        [Description("Kiadás raktár")]
        public Warehouse ToWarehouse { get; set; }

        [ForeignKey("UserID")]
        [ColumnLabel("Felhasználó")]
        [Description("Felhasználó")]
        public virtual Users User { get; set; }
    }
}
