using bbxBE.Application.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00014, "v00.00.01")]
    public class InitialTables_00014: Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {


            Execute.Sql(string.Format(@"
                if not exists (select * from Warehouse where WarehouseCode='{0}')
                begin
                    insert into Warehouse ([WarehouseCode],[WarehouseDescription])
                    values ('{0}','Relax központ (Szolnok)')
               end", 
                bbxBEConsts.DEF_WAREHOUSE));


            //valami ilyesmi kéne:    var WarehouseID =  Select.Table("Warehouse").Column("ID").Read();

            Execute.Sql(string.Format(@"
                if not exists (select * from Counter where CounterCode='{0}')
                begin
                    insert into Counter ([WarehouseID],[CounterCode],[CounterDescription],[Prefix],[CurrentNumber],[NumbepartLength],[Suffix],[CounterPool])
                    select ID, '{2}','{3}', '{4}', {5}, {6}, '{7}', '{8}' from Warehouse where WarehouseCode='{1}')
                end",
                "K"+enInvoiceType.INV.ToString()+ "_" + bbxBEConsts.DEF_WAREHOUSE,
                bbxBEConsts.DEF_WAREHOUSE, enInvoiceType.INV.ToString() + "_" + bbxBEConsts.DEF_WAREHOUSE, "Szolnok KP", "K-", 1, 5, "S", ""));

            Execute.Sql(string.Format(@"
                if not exists (select * from Counter where CounterCode='{0}')
                begin
                    insert into Counter ([WarehouseID],[CounterCode],[CounterDescription],[Prefix],[CurrentNumber],[NumbepartLength],[Suffix],[CounterPool])
                    select ID, '{2}','{3}', '{4}', {5}, {6}, '{7}', '{8}' from Warehouse where WarehouseCode='{1}')
               end",
               "K" + enInvoiceType.DLV.ToString() + "_" + bbxBEConsts.DEF_WAREHOUSE,
               bbxBEConsts.DEF_WAREHOUSE, enInvoiceType.DLV.ToString() + "_" + bbxBEConsts.DEF_WAREHOUSE, "Szolnok Száll", "S-", 1, 5, "S", ""));
        }
    }
}
