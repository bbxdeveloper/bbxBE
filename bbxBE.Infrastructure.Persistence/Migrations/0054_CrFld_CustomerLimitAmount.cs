using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00054, "v00.02.08-Customer.WarningLimit, Customer.MaxLimit")]
    public class InitialTables_00054 : Migration
    {
        public override void Down()
        {
            Delete.Column("WarningLimit").FromTable("Customer");
            Delete.Column("MaxLimit").FromTable("Customer");
        }
        public override void Up()
        {


            Alter.Table("Customer")
                .AddColumn("WarningLimit").AsDecimal();
            Alter.Table("Customer")
                .AddColumn("MaxLimit").AsDecimal();
        }
    }
}
