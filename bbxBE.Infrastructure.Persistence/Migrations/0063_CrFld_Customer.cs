using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00063, "v00.02.10 -Customer.LatestDiscountPercent")]
    public class InitialTables_00063 : Migration
    {
        public override void Down()
        {
            Delete.Column("LatestDiscountPercent").FromTable("Customer");
        }
        public override void Up()
        {
            Alter.Table("Customer")
                .AddColumn("LatestDiscountPercent").AsDecimal().Nullable();

            //        Execute.Sql($"update Customer set LatestDiscountPercent = 0");

        }
    }
}
