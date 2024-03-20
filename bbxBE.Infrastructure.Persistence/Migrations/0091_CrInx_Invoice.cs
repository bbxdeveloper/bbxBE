﻿using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00091, "v00.02.55-INX_InvoiceIncomingPaymentMethodCustomerID for unpaid invoices II.")]
    public class InitialTables_00091 : Migration
    {
        public override void Down()
        {
            Delete.Index("INX_InvoiceIncomingPaymentMethodInvoiceDeliveryDateCustomerID")
            .OnTable("Invoice");
        }
        public override void Up()
        {
            Delete.Index("INX_InvoiceIncomingPaymentMethodInvoiceDeliveryDateCustomerID")
            .OnTable("Invoice");

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