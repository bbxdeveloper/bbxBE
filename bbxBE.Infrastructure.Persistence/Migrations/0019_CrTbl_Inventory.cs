using bbxBE.Application.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00019, "v00.00.01-Inventory")]
    public class InitialTables_00019 : Migration
    {
        public override void Down()
        {
            Delete.Table("InvCtrlPeriod");

            /*
                delete VersionInfo where Version = 19
                drop table InvCtrlPeriod
             */
        }
        public override void Up()
        {

            Create.Table("InvCtrlPeriod")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)

                    .WithColumn("WarehouseID").AsInt64().ForeignKey()
                    .WithColumn("DateFrom").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("DateTo").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Closed").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("UserID").AsInt64().ForeignKey().Nullable().WithDefaultValue(0);
            


            Create.Index("INX_InvCtrlPeriod")
                         .OnTable("InvCtrlPeriod")
                         .OnColumn("WarehouseID").Ascending()
                         .OnColumn("DateFrom").Ascending()
                         .OnColumn("DateTo").Ascending()
                         .WithOptions().NonClustered();


        }
    }
}
