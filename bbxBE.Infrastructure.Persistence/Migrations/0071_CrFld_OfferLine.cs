using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00071, "v00.02.28-OfferLine.NoDiscount")]
    public class InitialTables_00071 : Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {
            Alter.Table("OfferLine")
                .AddColumn("NoDiscount").AsBoolean().NotNullable().WithDefaultValue(false);

            Execute.Sql(string.Format(@"update ol 
                set ol.NoDiscount = pr.NoDiscount
                from OfferLine ol
                inner join Product pr on pr.ID = ol.ProductID"));

        }
    }
}
