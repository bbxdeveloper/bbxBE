using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00005, "v00.00.01")]
    public class InitialTables_00005 : Migration
    {
        public override void Down()
        {
            Delete.Table("Warehouse");
        }
        public override void Up()
        {

            Create.Table("Warehouse")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                    .WithColumn("WarehouseCode").AsString().NotNullable()
                    .WithColumn("WarehouseDescription").AsString();

            Insert.IntoTable("Warehouse").Row(new { WarehouseCode = "001", WarehouseDescription = "Szolnok központi" });
        }

    }

}
