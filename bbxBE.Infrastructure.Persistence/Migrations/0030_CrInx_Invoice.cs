using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00030, "v00.00.01-Invoice indexes")]
    public class InitialTables_00030 : Migration
    {
        public override void Down()
        {
            Delete.Index("INX_InvoiceIncomingWarehouseTypeCurrencyCodeCustomerSupplier")
            .OnTable("Invoice");
        }
        public override void Up()
        {

            Create.Index("INX_InvoiceIncomingWarehouseTypeCurrencyCodeCustomerSupplier")
                         .OnTable("Invoice")
                         .OnColumn("Incoming").Ascending()
                         .OnColumn("WarehouseID").Ascending()
                         .OnColumn("InvoiceType").Ascending()
                         .OnColumn("CurrencyCode").Ascending()
                         .OnColumn("SupplierID").Ascending()
                         .OnColumn("CustomerID").Ascending()
                         .WithOptions().NonClustered();
        }
    }
}
