using bbxBE.Common;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(0040, "v00.01.95-DelFld_CalcQty_OutQty")]
    public class InitialTables_00040 : Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {


            Delete.Column("OCalcQty").FromTable("InvCtrl");
            Delete.Column("NCalcQty").FromTable("InvCtrl");

            Delete.Column("CalcQty").FromTable("Stock");
            Delete.Column("OutQty").FromTable("Stock");

            Delete.Column("OCalcQty").FromTable("StockCard");
            Delete.Column("XCalcQty").FromTable("StockCard");
            Delete.Column("NCalcQty").FromTable("StockCard");
            Delete.Column("OOutQty").FromTable("StockCard");
            Delete.Column("NOutQty").FromTable("StockCard");
            Delete.Column("XOutQty").FromTable("StockCard");

        }
    }
}
