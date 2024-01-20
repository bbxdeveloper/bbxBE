using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00081, "v00.00.38 InvoiceLine.UnitWeight")]
    public class InitialTables_00081 : Migration
    {
        public override void Down()
        {
            Delete.Column("UnitWeight").FromTable("InvoiceLine");
        }
        public override void Up()
        {
            Alter.Table("InvoiceLine")
                .AddColumn("UnitWeight").AsDecimal().NotNullable().ForeignKey().WithDefaultValue(0);
            Update.Table("InvoiceLine").Set(new { UnitWeight = 0 }).AllRows();
        }
    }
}
