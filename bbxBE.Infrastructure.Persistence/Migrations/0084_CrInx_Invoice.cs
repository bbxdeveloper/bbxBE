using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00084, "v00.02.41-INX_InvoiceIncomingPaymentMethodInvoiceIssueDate for unpaid invoices")]
    public class InitialTables_00084 : Migration
    {
        public override void Down()
        {
            Delete.Index("INX_InvoiceIncomingPaymentMethodInvoiceIssueDate")
            .OnTable("Invoice");
        }
        public override void Up()
        {

            Create.Index("INX_InvoiceIncomingPaymentMethodInvoiceIssueDate")
                         .OnTable("Invoice")
                         .OnColumn("Incoming").Ascending()
                         .OnColumn("PaymentMethod").Ascending()
                         .OnColumn("InvoiceIssueDate").Ascending()
                         .WithOptions().NonClustered();
        }
    }
}
