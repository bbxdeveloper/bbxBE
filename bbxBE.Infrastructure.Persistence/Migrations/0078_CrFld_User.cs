using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00078, "v00.02.37 - User.WarehouseID")]
    public class InitialTables_00078 : Migration
    {
        public override void Down()
        {
            Delete.Column("WarehouseID").FromTable("Users");
        }
        public override void Up()
        {
            Alter.Table("Users")
                .AddColumn("WarehouseID").AsInt64().Nullable().ForeignKey();

        }
    }
}
