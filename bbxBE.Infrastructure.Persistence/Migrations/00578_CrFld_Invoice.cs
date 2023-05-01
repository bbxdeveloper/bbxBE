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
            Rename.Column("Invoice").OnTable("Correction").To("InvoiceCorrection");
        }
    }
}
