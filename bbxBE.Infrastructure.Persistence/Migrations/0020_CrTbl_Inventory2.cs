using bbxBE.Application.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00020, "v00.00.01-Inventory2")]
    public class InitialTables_00020 : Migration
    {
        public override void Down()
        {
            Delete.Table("InvCtrl");

            /*
                delete VersionInfo where Version = 20
                drop table InvCtrl
             */
        }
        public override void Up()
        {

            Create.Table("InvCtrl")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)

                    .WithColumn("WarehouseID").AsInt64().ForeignKey()
                    .WithColumn("InvCtlPeriodID").AsInt64().Nullable().ForeignKey()     //Opcionális, mert később lehetséges a folyamatos leltár beveeztése is!
                    .WithColumn("ProductID").AsInt64().ForeignKey()
                    .WithColumn("ProductCode").AsString()

                    .WithColumn("InvCtrlDate").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)

                    .WithColumn("OCalcQty").AsDecimal().NotNullable().WithDefaultValue(0)
                    .WithColumn("ORealQty").AsDecimal().NotNullable().WithDefaultValue(0)

                    .WithColumn("NCalcQty").AsDecimal().NotNullable().WithDefaultValue(0)
                    .WithColumn("NRealQty").AsDecimal().NotNullable().WithDefaultValue(0)

                    .WithColumn("AvgCost").AsDecimal().NotNullable().WithDefaultValue(0)        //átlagolt beszerzési egységár

                    .WithColumn("UserID").AsInt64().Nullable().WithDefaultValue(0);
            


            Create.Index("INX_InvCtrl")
                         .OnTable("InvCtrlPeriod")
                         .OnColumn("WarehouseID").Ascending()
                         .OnColumn("ProductID").Ascending()
                         .OnColumn("InvCtlPeriodID").Ascending()     //Opcionális, mert később lehetséges a folyamatos leltár beveeztése is!
                         .WithOptions().NonClustered();


        }
    }
}
