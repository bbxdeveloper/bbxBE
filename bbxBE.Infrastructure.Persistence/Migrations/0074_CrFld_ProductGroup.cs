using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(0074, "v00.02.33 - ProductGroup.MinMargin")]
    public class InitialTables_00074 : Migration
    {
        public override void Down()
        {
            Delete.Column("MinMargin").FromTable("ProductGroup");
        }
        public override void Up()
        {
            Alter.Table("ProductGroup")
                .AddColumn("MinMargin").AsDecimal().Nullable();

            //        Execute.Sql($"update Customer set LatestDiscountPercent = 0");

        }
    }
}
