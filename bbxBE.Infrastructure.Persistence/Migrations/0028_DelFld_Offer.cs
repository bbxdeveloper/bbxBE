using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00028, "v00.01.64-Offer del fields: UnitVat, UnitPriceHUF,UnitVatHUF, UnitGrossHUF")]
    public class InitialTables_00028 : Migration
    {
        public override void Down()
        {

        }
        public override void Up()
        {
            Delete.Column("UnitPriceHUF").FromTable("OfferLine");
            Delete.Column("UnitVat").FromTable("OfferLine");
            Delete.Column("UnitVatHUF").FromTable("OfferLine");
            Delete.Column("UnitGrossHUF").FromTable("OfferLine");
        }
    }
}
