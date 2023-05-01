using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00058, "v00.02.08 -Invoice Correction -> InvoiceCorrection")]
    public class InitialTables_00058 : Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {
            Alter.Table("Invoice")
                .AddColumn("InvoiceCorrection").AsBoolean().NotNullable().WithDefaultValue(false);

            Execute.Sql(string.Format(@"update Invoice set InvoiceCorrection = Correction"));

            Delete.Column("Correction").FromTable("Invoice");
        }
    }
}
