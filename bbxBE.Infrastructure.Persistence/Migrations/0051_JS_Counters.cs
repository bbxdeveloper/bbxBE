using bbxBE.Common.Consts;
using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(0051, "v00.02.04-JS counters")]
    public class InitialTables_00051 : Migration
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
               "JSK_" + bbxBEConsts.DEF_WAREHOUSE,
               bbxBEConsts.DEF_WAREHOUSE, "JSK_" + bbxBEConsts.DEF_WAREHOUSE, "Szolnok kimenő javítószámla", "JSK", 0, 5, "/", ""));

            Execute.Sql(string.Format(@"
                if not exists (select * from Counter where CounterCode='{0}')
                begin
                    insert into Counter ([WarehouseID],[CounterCode],[CounterDescription],[Prefix],[CurrentNumber],[NumbepartLength],[Suffix],[CounterPool])
                    select ID, '{2}','{3}', '{4}', {5}, {6}, '{7}', '{8}' from Warehouse where WarehouseCode='{1}'
               end",
               "JSB_" + bbxBEConsts.DEF_WAREHOUSE,
               bbxBEConsts.DEF_WAREHOUSE, "JSB_" + bbxBEConsts.DEF_WAREHOUSE, "Szolnok bejövő javítószámla", "JSB", 0, 5, "/", ""));

        }
    }
}
