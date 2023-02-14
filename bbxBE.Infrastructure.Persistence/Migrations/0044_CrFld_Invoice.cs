using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00044, "v00.01.101-Invoice DeliveryNote fields renaming")]
    public class InitialTables_00044: Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {
            Execute.Sql(string.Format(@"update il set il.LineDeliveryDate = i.InvoiceDeliveryDate,
                        il.LineExchangeRate = i.ExchangeRate
                        from InvoiceLine il
                        inner join Invoice i on il.InvoiceId = i.ID"));

            Delete.Index("INX_OrigDeliveryNoteInvoiceID").OnTable("InvoiceLine");

            Delete.Column("OrigDeliveryNoteNumber").FromTable("InvoiceLine");
            Delete.Column("OrigDeliveryNoteInvoiceID").FromTable("InvoiceLine");
            Delete.Column("OrigDeliveryNoteInvoiceLineID").FromTable("InvoiceLine");


            Alter.Table("InvoiceLine")
                .AddColumn("RelDeliveryNoteNumber").AsString().Nullable();
            Alter.Table("InvoiceLine")
                .AddColumn("RelDeliveryNoteInvoiceID").AsInt64().Nullable().ForeignKey();
            Alter.Table("InvoiceLine")
                .AddColumn("RelDeliveryNoteInvoiceLineID").AsInt64().Nullable().ForeignKey();


            Create.Index("INX_RelDeliveryNoteInvoiceID")
                           .OnTable("InvoiceLine")
                           .OnColumn("RelDeliveryNoteInvoiceID").Ascending()
                           .WithOptions().NonClustered();


        }
    }
}
