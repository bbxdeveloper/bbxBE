using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00022, "v00.00.01-InvCtrl.StockID")]
    public class InitialTables_00022 : Migration
    {
        public override void Down()
        {
            Delete.Column("StockID").FromTable("InvCtrl");
        }
        public override void Up()
        {

            Alter.Table("InvCtrl")
                .AddColumn("StockID").AsInt64().Nullable().ForeignKey();

            /*
                    Execute.Sql(string.Format(@"update InvCtrl
                            set StockID = (select top 1 ID from Stock where Stock.ProductID = InvCtrl.ProductID and Stock.WarehouseID = InvCtrl.WarehouseID)
                            where InvCtrl.InvCtrlType = '{0}'",
                        enInvCtrlType.ICP.ToString()));
            */
        }
    }
}
