using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00011, "v00.00.01")]
    public class InitialTables_00011 : Migration
    {
        public override void Down()
        {
            Delete.Column("VatRateID").FromTable("Product");
        }
        public override void Up()
        {


            Alter.Table("Product")
                .AddColumn("VatRateID").AsInt64().NotNullable().ForeignKey().WithDefaultValue(1);
            Update.Table("Product").Set(new { VatRateID = 1 }).AllRows();
        }
    }
}
