using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00021, "v00.00.01-Stock indexes")]
    public class InitialTables_00021 : Migration
    {
        public override void Down()
        {
            Delete.Index("INX_WarehouseProduct")
            .OnTable("Stock");
        }
        public override void Up()
        {

            Create.Index("INX_WarehouseProduct")
                         .OnTable("Stock")
                         .OnColumn("WarehouseID").Ascending()
                         .OnColumn("ProductID").Ascending()
                         .WithOptions().NonClustered();
        }
    }
}
