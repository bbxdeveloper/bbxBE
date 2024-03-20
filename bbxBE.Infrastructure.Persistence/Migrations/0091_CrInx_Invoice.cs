using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00091, "v00.02.55-INX_InvoiceIncomingPaymentMethodCustomerID for unpaid invoices II.")]
    public class InitialTables_00091 : Migration
    {
        public override void Down()
        {
            Delete.Index("INX_InvoiceIncomingPaymentMethodCustomerID")
            .OnTable("Invoice");
        }
        public override void Up()
        {

            var sql = "IF  EXISTS (SELECT * FROM sys.indexes WHERE NAME = N'INX_InvoiceIncomingPaymentMethodInvoiceDeliveryDateCustomerID') " +
                      " DROP INDEXINX_InvoiceIncomingPaymentMethodInvoiceDeliveryDateCustomerID ON Invoice";
            Execute.Sql(sql);

            Create.Index("INX_InvoiceIncomingPaymentMethodCustomerID")
                         .OnTable("Invoice")
                         .OnColumn("Incoming").Ascending()
                         .OnColumn("PaymentMethod").Ascending()
                         .OnColumn("CustomerID").Ascending()
                         .OnColumn("SupplierID").Ascending()
                         .WithOptions().NonClustered();
        }
    }
}
