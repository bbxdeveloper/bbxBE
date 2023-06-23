using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00065, "v00.02.12-StockCard indexes II")]
    public class InitialTables_00065 : Migration
    {
        public override void Down()
        {
            Delete.Index("INX_StockCardWhsProdDateScType")
            .OnTable("StockCard");
        }
        public override void Up()
        {

            Create.Index("INX_StockCardWhsProdDateScType")
                         .OnTable("StockCard")
                         .OnColumn("WarehouseID").Ascending()
                         .OnColumn("ProductID").Ascending()
                         .OnColumn("StockCardDate").Ascending()
                         .OnColumn("ScType").Ascending()
                         .WithOptions().NonClustered();
        }
    }
}
