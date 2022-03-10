using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00003, "v00.00.01")]
    public class InitialTables_00003 : Migration
    {
        public override void Down()
        {
            Delete.Table("ProductGroup");
            Delete.Table("Origin");
        }
        public override void Up()
        {

            Create.Table("ProductGroup")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                    .WithColumn("ProductGroupCode").AsString().NotNullable()
                    .WithColumn("ProductGroupDescription").AsString();

            Create.Table("Origin")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                    .WithColumn("OriginCode").AsString().NotNullable()
                    .WithColumn("OriginDescription").AsString();
        }

    }

}
