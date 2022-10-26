using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00032, "v00.00.01-Product.NoDiscount")]
    public class InitialTables_00032 : Migration
    {
        public override void Down()
        {
            Delete.Column("IsOwnData").FromTable("Product");
        }
        public override void Up()
        {
            Alter.Table("Product")
                .AddColumn("NoDiscount").AsBoolean().NotNullable().WithDefaultValue(false);
            Update.Table("Product").Set(new { NoDiscount = false }).AllRows();
        }
    }
}
