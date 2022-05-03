using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00013, "v00.00.01")]
    public class InitialTables_00013 : Migration
    {
        public override void Down()
        {
            Delete.Column("CounterPool").FromTable("Counter");
        }
        public override void Up()
        {


            Alter.Table("Counter")
                .AddColumn("CounterPool").AsString(int.MaxValue).WithDefaultValue("");
            Update.Table("Counter").Set(new { CounterPool = "" }).AllRows();
        }
    }
}
