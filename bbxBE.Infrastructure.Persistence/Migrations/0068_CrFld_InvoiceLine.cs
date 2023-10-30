using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00068, "v00.02.26 -InvoiceLine:delete unused fields")]
    public class InitialTables_00068 : Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {
            Delete.Column("TakeoverProductCodeCategory").FromTable("InvoiceLine");
            Delete.Column("TakeoverProductCodeValue").FromTable("InvoiceLine");
        }
    }
}
