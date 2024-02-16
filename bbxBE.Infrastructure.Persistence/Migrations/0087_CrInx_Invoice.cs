using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00087, "v00.02.52-INX_InvoiceIncomingPaymentMethodInvoiceDeliveryDateCustomerID for unpaid invoices II.")]
    public class InitialTables_00087 : Migration
    {
        public override void Down()
        {
            Delete.Index("INX_InvoiceIncomingPaymentMethodInvoiceDeliveryDateCustomerID")
            .OnTable("Invoice");
        }
        public override void Up()
        {
            Delete.Index("INX_InvoiceIncomingPaymentMethodInvoiceIssueDate")
            .OnTable("Invoice");

            Create.Index("INX_InvoiceIncomingPaymentMethodInvoiceDeliveryDateCustomerID")
                         .OnTable("Invoice")
                         .OnColumn("Incoming").Ascending()
                         .OnColumn("PaymentMethod").Ascending()
                         .OnColumn("InvoiceDeliveryDate").Ascending()
                         .OnColumn("CustomerID").Ascending()
                         .OnColumn("SupplierID").Ascending()
                         .WithOptions().NonClustered();
        }
    }
}
