using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00046, "v00.02.08-InvoiceLine.Correction")]
    public class InitialTables_00046: Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {
            Alter.Table("Invoice")
                .AddColumn("Correction").AsBoolean().NotNullable().WithDefaultValue(false);

            Execute.Sql(string.Format(@"update Invoice set Correction = 0"));

        }
    }
}
