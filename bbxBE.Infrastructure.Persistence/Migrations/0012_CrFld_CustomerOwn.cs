using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00012, "v00.00.01")]
    public class InitialTables_00012 : Migration
    {
        public override void Down()
        {
            Delete.Column("IsOwnData").FromTable("Customer");
        }
        public override void Up()
        {


            Alter.Table("Customer")
                .AddColumn("IsOwnData").AsBoolean().NotNullable().WithDefaultValue(false);
            Update.Table("Customer").Set(new { IsOwnData = false }).AllRows();
        }
    }
}
