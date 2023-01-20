using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00042, "v00.01.96-Stock.Location")]
    public class InitialTables_00042 : Migration
    {
        public override void Down()
        {
            Delete.Column("LocationID").FromTable("Stock");
        }
        public override void Up()
        {

            Alter.Table("Stock")
                   .AddColumn("LocationID").AsDecimal().Nullable();
        }
    }
}
