using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00056, "v00.02.08-InvoiceLine indexes")]
    public class InitialTables_00056 : Migration
    {
        public override void Down()
        {
            Delete.Index("INX_InvoiceLineInvoiceID")
            .OnTable("InvoiceLine");
        }
        public override void Up()
        {

            Create.Index("INX_InvoiceLineInvoiceID")
                         .OnTable("InvoiceLine")
                         .OnColumn("InvoiceID").Ascending()
                         .OnColumn("LineNumber").Ascending()
                         .WithOptions().NonClustered();
        }
    }
}
