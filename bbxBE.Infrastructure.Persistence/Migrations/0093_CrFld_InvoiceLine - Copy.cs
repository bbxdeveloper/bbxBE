using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00093, "v00.02.56-InvoiceLine.POrderLineID")]
    public class InitialTables_00093 : Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {
            Alter.Table("InvoiceLine")
                .AddColumn("POrdereLineID").AsInt64().Nullable().ForeignKey()
                .AddColumn("POrderNumber").AsString();

        }
    }
}
