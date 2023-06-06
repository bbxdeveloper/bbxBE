using bbxBE.Common.Attributes;
using bbxBE.Common.Enums;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
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

        [ColumnLabel("Kiadás dátuma")]
        [Description("Kiadás dátuma")]
        public DateTime TransferDate { get; set; }

        [ColumnLabel("Bevétel dátuma")]
        [Description("Bevétel dátuma")]
        public DateTime? TransferDateIn { get; set; }

        [ColumnLabel("Megjegyzés")]
        [Description("Megjegyzés")]
        public string Notice { get; set; }

        private enWhsTransferStatus whsTransferStatus;
        [ColumnLabel("Státusz")]
        [Description("Státusz")]
        public string WhsTransferStatus
        {
            get { return Enum.GetName(typeof(enWhsTransferStatus), whsTransferStatus); }
            set
            {
                if (value != null)
                    whsTransferStatus = (enWhsTransferStatus)Enum.Parse(typeof(enWhsTransferStatus), value);
                else
                    whsTransferStatus = enWhsTransferStatus.READY;

            }
        }

        [ColumnLabel("Példány")]
        [Description("Nyomtatott példány száma")]
        public short Copies { get; set; }

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

        [ColumnLabel("Bizonylatsorok")]
        [Description("Bizonylatsorok")]
        public virtual List<WhsTransferLine> WhsTransferLines { get; set; }


    }
}
