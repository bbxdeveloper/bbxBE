using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00073, "v00.02.31-StockCard indexes III")]
    public class InitialTables_00073 : Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {
            Delete.Index("INX_StockCardStockCardDate")
                     .OnTable("StockCard");

            Delete.Index("INX_StockCardDate")
                        .OnTable("StockCard");

            Create.Index("INX_StockCardDateID")
                   .OnTable("StockCard")
                   .OnColumn("StockCardDate").Ascending()
                   .OnColumn("ID").Ascending()
                   .WithOptions().NonClustered();

            Create.Index("INX_StockCardDateID_Desc")
                   .OnTable("StockCard")
                   .OnColumn("StockCardDate").Descending()
                   .OnColumn("ID").Descending()
                   .WithOptions().NonClustered();
        }
    }
}
