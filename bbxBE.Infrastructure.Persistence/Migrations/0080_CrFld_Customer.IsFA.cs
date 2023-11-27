using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00080, "v00.02.38 - Customer.IsFA")]
    public class InitialTables_00080 : Migration
    {
        public override void Down()
        {
            Delete.Column("IsFA").FromTable("Customer");
        }
        public override void Up()
        {
            Alter.Table("Customer")
               .AddColumn("IsFA").AsBoolean().NotNullable().WithDefaultValue(false);
            Update.Table("Customer").Set(new { IsFA = false }).AllRows();
        }
    }
}
