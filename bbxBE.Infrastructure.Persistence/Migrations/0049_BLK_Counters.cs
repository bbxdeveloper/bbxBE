using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(0049, "v00.02.04-BLK counters")]
    public class InitialTables_00049 : Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {

            Execute.Sql(string.Format(@"
                if not exists (select * from Counter where CounterCode='{0}')
                begin
                    insert into Counter ([WarehouseID],[CounterCode],[CounterDescription],[Prefix],[CurrentNumber],[NumbepartLength],[Suffix],[CounterPool])
                    select ID, '{2}','{3}', '{4}', {5}, {6}, '{7}', '{8}' from Warehouse where WarehouseCode='{1}'
               end",
               "BLK_" + bbxBEConsts.DEF_WAREHOUSE,
               bbxBEConsts.DEF_WAREHOUSE, "BLK_" + bbxBEConsts.DEF_WAREHOUSE, "Szolnok kp. blokk", "BLK", 0, 5, "/", ""));

            Execute.Sql(string.Format(@"
                if not exists (select * from Counter where CounterCode='{0}')
                begin
                    insert into Counter ([WarehouseID],[CounterCode],[CounterDescription],[Prefix],[CurrentNumber],[NumbepartLength],[Suffix],[CounterPool])
                    select ID, '{2}','{3}', '{4}', {5}, {6}, '{7}', '{8}' from Warehouse where WarehouseCode='{1}'
               end",
               "BLC_" + bbxBEConsts.DEF_WAREHOUSE,
               bbxBEConsts.DEF_WAREHOUSE, "BLC_" + bbxBEConsts.DEF_WAREHOUSE, "Szolnok kártya blokk", "BLC", 0, 5, "/", ""));
        }
    }
}
