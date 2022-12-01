using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00037, "v00.01.92-Location")]
    public class InitialTables_00037 : Migration
    {
        public override void Down()
        {
            Delete.Table("Location");
        }
        public override void Up()
        {

            Create.Table("Location")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                    .WithColumn("LocationCode").AsString().NotNullable()
                    .WithColumn("LocationDescription").AsString();
        }

    }

}
