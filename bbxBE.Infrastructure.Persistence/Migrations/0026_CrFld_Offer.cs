using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00026, "v00.01.64-Offer new field: Quantity")]
    public class InitialTables_00026 : Migration
    {
        public override void Down()
        {
            Delete.Column("Quantity").FromTable("OfferLine");

        }
        public override void Up()
        {
            Alter.Table("OfferLine")
                .AddColumn("Quantity").AsDecimal().NotNullable().WithDefaultValue(0);
        }
    }
}
