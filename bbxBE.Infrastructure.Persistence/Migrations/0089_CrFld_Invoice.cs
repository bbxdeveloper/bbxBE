using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00089, "v00.02.53-Invvoice: Imported mező")]
    public class InitialTables_00089 : Migration
    {
        public override void Down()
        {
            Delete.Column("Imported").FromTable("Invoice");
        }
        public override void Up()
        {
            Alter.Table("Invoice")
                    .AddColumn("Imported").AsBoolean().WithDefaultValue(false);
        }
    }
}
