using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00024, "v00.02.10-StockCard indexes")]
    public class InitialTables_00064 : Migration
    {
        public override void Down()
        {
            Delete.Index("INX_StockCardStockCardDate")
            .OnTable("StockCard");
        }
        public override void Up()
        {

            Create.Index("INX_StockCardStockCardDate")
                         .OnTable("StockCard")
                         .OnColumn("StockCardDate").Descending()
                         .OnColumn("ID").Descending()
                         .WithOptions().NonClustered();
        }
    }
}
