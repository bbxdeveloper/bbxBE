using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00092, "v00.02.56-POrder counters")]
    public class InitialTables_00092 : Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {
            var sql = "INSERT INTO[dbo].[Counter]([WarehouseID], [CounterCode] ,[CounterDescription] ,[Prefix] ,[CurrentNumber],[NumbepartLength] ,[Suffix]) \n" +
                      "select ID as WarehouseID, 'SZM_00' + cast(ID as char) as CounterCode, 'Szállítói megrendelés' as CounterDescription, \n" +
                      "'SZM' as Prefix, 0 as CurrentNumber, 4 as NumbepartLength, '/' as Suffix from Warehouse";
            Execute.Sql(sql);

        }
    }
}
