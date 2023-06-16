using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00062, "v00.02.08 -StockCard.InvCtrlID, StockCard.WhsTransferLineID")]
    public class InitialTables_00062 : Migration
    {
        public override void Down()
        {
            Delete.Column("InvCtrlID").FromTable("StockCard");
            Delete.Column("WhsTransferLineID").FromTable("StockCard");
        }
        public override void Up()
        {
            Alter.Table("StockCard")
                .AddColumn("InvCtrlID").AsInt64().Nullable().ForeignKey()
                .AddColumn("WhsTransferLineID").AsInt64().Nullable().ForeignKey();

        }
    }
}
