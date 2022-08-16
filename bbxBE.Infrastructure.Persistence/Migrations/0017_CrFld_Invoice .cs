using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00017, "v00.00.01-Invoice")]
    public class InitialTables_00017 : Migration
    {
        public override void Down()
        {
            Delete.Index("INX_CustomerInvoiceNumber").OnTable("Invoice");
            Delete.Column("CustomerInvoiceNumber").FromTable("Invoice");
        }
        public override void Up()
        {
            Alter.Table("Invoice")
                .AddColumn("CustomerInvoiceNumber").AsString().Nullable().WithDefaultValue("");

            Create.Index("INX_CustomerInvoiceNumber")
                         .OnTable("Invoice")
                         .OnColumn("CustomerInvoiceNumber").Ascending()
                         .WithOptions().NonClustered();

            Execute.Sql(string.Format(@"
                if not exists (select * from Counter where CounterCode='{0}')
                begin
                    insert into Counter ([WarehouseID],[CounterCode],[CounterDescription],[Prefix],[CurrentNumber],[NumbepartLength],[Suffix],[CounterPool])
                    select ID, '{2}','{3}', '{4}', {5}, {6}, '{7}', '{8}' from Warehouse where WarehouseCode='{1}'
               end",
               "B" + enInvoiceType.INC.ToString() + "_" + bbxBEConsts.DEF_WAREHOUSE,
               bbxBEConsts.DEF_WAREHOUSE, "B" + enInvoiceType.INC.ToString() + "_" + bbxBEConsts.DEF_WAREHOUSE, "Szolnok bevételezés", "BEV-", 1, 5, "S", ""));
        }
    }
}
