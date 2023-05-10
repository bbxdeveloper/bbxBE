using bbxBE.Common.Consts;
using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(0059, "v00.02.08-WHT counters")]
    public class InitialTables_00059 : Migration
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
               "WHT_" + bbxBEConsts.DEF_WAREHOUSE,
               bbxBEConsts.DEF_WAREHOUSE, "WHT_" + bbxBEConsts.DEF_WAREHOUSE, "Szolnok raktárközi", "WHT", 0, 5, "/", ""));

        }
    }
}
