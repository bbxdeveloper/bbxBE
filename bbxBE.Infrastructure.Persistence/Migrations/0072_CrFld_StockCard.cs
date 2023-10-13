using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00072, "v00.02.29 - StockCard.Correction")]
    public class InitialTables_00072 : Migration
    {
        public override void Down()
        {
            Delete.Column("Correction").FromTable("StockCard");
        }
        public override void Up()
        {
            Alter.Table("StockCard")
                .AddColumn("Correction").AsBoolean().Nullable();

            Execute.Sql(string.Format(@"update sc set sc.Correction = 0 from StockCard sc where sc.Correction is null"));

            Alter.Table("StockCard")
                .AlterColumn("Correction").AsBoolean().NotNullable();

        }
    }
}
