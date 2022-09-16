using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(0025, "v00.00.01-Invoice - Delivery Note")]
    public class InitialTables_00025 : Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {

            Delete.Column("DeliveryNote").FromTable("InvoiceLine");
            Delete.Column("DeliveryNoteInvoiceID").FromTable("InvoiceLine");
            Delete.Column("DeliveryNoteLineNumber").FromTable("InvoiceLine");

            Alter.Table("InvoiceLine")
                .AddColumn("OrigDeliveryNoteNumber").AsString().Nullable();
            Alter.Table("InvoiceLine")
                .AddColumn("OrigDeliveryNoteInvoiceID").AsInt64().Nullable().ForeignKey();
            Alter.Table("InvoiceLine")
                .AddColumn("OrigDeliveryNoteInvoiceLineID").AsInt64().Nullable().ForeignKey();
            Alter.Table("InvoiceLine")
                .AddColumn("PendingDNQuantity").AsDecimal().WithDefaultValue(0);


            Create.Index("INX_DeliveryNoteInvoiceID")
                           .OnTable("InvoiceLine")
                           .OnColumn("OrigDeliveryNoteInvoiceID").Ascending()
                           .OnColumn("PendingDNQuantity").Descending()
                           .WithOptions().NonClustered();
            Create.Index("INX_DeliveryNoteInvoiceLineID")
                           .OnTable("InvoiceLine")
                           .OnColumn("OrigDeliveryNoteInvoiceID").Ascending()
                           .OnColumn("PendingDNQuantity").Descending()
                           .WithOptions().NonClustered();


            //Régi szolnoki szállítólevélszám counter törlése
            Execute.Sql(string.Format(@"
                if not exists (select * from Counter where CounterCode='KDLV_001')
                begin
                     delete Counter where CounterCode='KDLV_001'
               end"));

            Execute.Sql(string.Format(@"
                if not exists (select * from Counter where CounterCode='{0}')
                begin
                    insert into Counter ([WarehouseID],[CounterCode],[CounterDescription],[Prefix],[CurrentNumber],[NumbepartLength],[Suffix],[CounterPool])
                    select ID, '{2}','{3}', '{4}', {5}, {6}, '{7}', '{8}' from Warehouse where WarehouseCode='{1}'
               end",
               "K" + enInvoiceType.DNO.ToString() + "_" + bbxBEConsts.DEF_WAREHOUSE,
               bbxBEConsts.DEF_WAREHOUSE, "K" + enInvoiceType.DNO.ToString() + "_" + bbxBEConsts.DEF_WAREHOUSE, "Szolnok szállító", "S-", 1, 5, "/", ""));


            Execute.Sql(string.Format(@"
                if not exists (select * from Counter where CounterCode='{0}')
                begin
                    insert into Counter ([WarehouseID],[CounterCode],[CounterDescription],[Prefix],[CurrentNumber],[NumbepartLength],[Suffix],[CounterPool])
                    select ID, '{2}','{3}', '{4}', {5}, {6}, '{7}', '{8}' from Warehouse where WarehouseCode='{1}'
               end",
               "B" + enInvoiceType.DNI.ToString() + "_" + bbxBEConsts.DEF_WAREHOUSE,
               bbxBEConsts.DEF_WAREHOUSE, "B" + enInvoiceType.DNI.ToString() + "_" + bbxBEConsts.DEF_WAREHOUSE, "Szolnok bev. szállító", "BEVS-", 1, 5, "/", ""));


            Delete.Index("INX_StockCardDate")
                        .OnTable("StockCard");

            Create.Index("INX_StockCardDate")
                   .OnTable("StockCard")
                   .OnColumn("StockCardDate").Descending()
                   .OnColumn("ID").Descending()
                   .WithOptions().NonClustered();

        }
    }
}
