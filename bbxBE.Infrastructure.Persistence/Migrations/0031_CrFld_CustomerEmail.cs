using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00031, "v00.00.01-Customer.Email")]
    public class InitialTables_00031 : Migration
    {
        public override void Down()
        {
            Delete.Column("Email").FromTable("Customer");
        }
        public override void Up()
        {


            Alter.Table("Customer")
                .AddColumn("Email").AsString().Nullable();
        }
    }
}
