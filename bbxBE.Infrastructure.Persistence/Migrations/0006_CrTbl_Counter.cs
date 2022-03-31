using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00006, "v00.00.01")]
    public class InitialTables_00006 : Migration
    {
        public override void Down()
        {
            Delete.Table("Counter");
        }
        public override void Up()
        {

            Create.Table("Counter")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                    .WithColumn("WarehouseID").AsInt64().Nullable().ForeignKey()
                    .WithColumn("CounterCode").AsString().NotNullable()
                    .WithColumn("CounterDescription").AsString()
                    .WithColumn("Prefix").AsString().NotNullable()
                    .WithColumn("CurrentNumber").AsInt64().NotNullable()
                    .WithColumn("NumbepartLength").AsInt32().NotNullable()
                    .WithColumn("Suffix").AsString().NotNullable();

        }

    }

}
