using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00052, "v00.02.04-Invoice indexes")]
    public class InitialTables_00052 : Migration
    {
        public override void Down()
        {
            Delete.Index("INX_InvoiceOriginalInvoiceID")
            .OnTable("Invoice");
        }
        public override void Up()
        {

            Create.Index("INX_InvoiceOriginalInvoiceID")
                         .OnTable("Invoice")
                         .OnColumn("OriginalInvoiceID").Ascending()
                         .WithOptions().NonClustered();
        }
    }
}
