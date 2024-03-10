using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00090, "v00.02.53-INX_InvoiceIncomingInvoiceTypeImportedInvoiceDeliveryDate")]
    public class InitialTables_00090 : Migration
    {
        public override void Down()
        {
            Delete.Index("INX_InvoiceIncomingInvoiceTypeImportedInvoiceDeliveryDate")
            .OnTable("Invoice");
        }
        public override void Up()
        {

            Create.Index("INX_InvoiceIncomingInvoiceTypeImportedInvoiceDeliveryDate")
                         .OnTable("Invoice")
                         .OnColumn("Incoming").Ascending()
                         .OnColumn("InvoiceType").Ascending()
                         .OnColumn("Imported").Ascending()
                         .OnColumn("InvoiceDeliveryDate").Ascending()
                         .WithOptions().NonClustered();
        }
    }
}
