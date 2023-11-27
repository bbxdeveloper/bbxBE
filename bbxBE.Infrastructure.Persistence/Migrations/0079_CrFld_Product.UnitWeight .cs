using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00079, "v00.00.38 Product.UnitWeight")]
    public class InitialTables_00079 : Migration
    {
        public override void Down()
        {
            Delete.Column("UnitWeight").FromTable("Product");
        }
        public override void Up()
        {
            Alter.Table("Product")
                .AddColumn("UnitWeight").AsDecimal().NotNullable().ForeignKey().WithDefaultValue(0);
            Update.Table("Product").Set(new { UnitWeight = 0 }).AllRows();
        }
    }
}
