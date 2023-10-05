using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00070, "v00.02.28-OfferLine new field: OriginalUnitPriceHUF")]
    public class InitialTables_00070 : Migration
    {
        public override void Down()
        {
            Delete.Column("OriginalUnitPriceHUF").FromTable("OfferLine");

        }
        public override void Up()
        {
            Alter.Table("OfferLine")
                .AddColumn("OriginalUnitPriceHUF").AsCurrency().NotNullable().WithDefaultValue(0);
            Execute.Sql(@"update OfferLine set OriginalUnitPriceHUF=OriginalUnitPrice");
            Execute.Sql(@"update ol set ol.OriginalUnitPrice=round(ol.OriginalUnitPriceHUF/o.ExchangeRate,2) from OfferLine ol inner join Offer o on o.ID=ol.OfferID where o.ExchangeRate <> 0");

        }
    }
}
