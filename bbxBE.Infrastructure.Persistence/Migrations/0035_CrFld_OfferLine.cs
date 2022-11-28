using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00035, "v00.01.88-OfferLine new fields: OriginalUnitPrice,UnitPriceSwitch")]
    public class InitialTables_00035 : Migration
    {
        public override void Down()
        {
            Delete.Column("OriginalUnitPrice").FromTable("OfferLine");
            Delete.Column("UnitPriceSwitch").FromTable("OfferLine");

        }
        public override void Up()
        {
            Alter.Table("OfferLine")
                .AddColumn("OriginalUnitPrice").AsCurrency().NotNullable().WithDefaultValue(0);
            Alter.Table("OfferLine")
                .AddColumn("UnitPriceSwitch").AsBoolean().NotNullable().WithDefaultValue(false);
        }
    }
}
