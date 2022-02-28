using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00004, "v00.00.01")]
    public class InitialTables_00004 : Migration
    {
        public override void Down()
        {
            Delete.Table("ProductCode");
            Delete.Table("Product");
        }
        public override void Up()
        {



            Create.Table("ProductCode")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                    .WithColumn("ProductID").AsInt64().NotNullable().ForeignKey()
                    .WithColumn("ProductCodeCategory").AsString().NotNullable()
                    .WithColumn("ProductCodeValue").AsString().NotNullable();


            Create.Table("Product")
                 .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                 .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                 .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                 .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                 .WithColumn("Description").AsString().NotNullable()
                 .WithColumn("ProductGroupID").AsInt64().NotNullable().ForeignKey()
                 .WithColumn("OriginID").AsInt64().NotNullable().ForeignKey()
                 .WithColumn("UnitOfMeasure").AsString().NotNullable()
                 .WithColumn("UnitPrice1").AsCurrency().NotNullable()
                 .WithColumn("UnitPrice2").AsCurrency()
                 .WithColumn("LatestSupplyPrice").AsCurrency()
                 .WithColumn("IsStock").AsBoolean().NotNullable()
                 .WithColumn("MinStock").AsDecimal()
                 .WithColumn("OrdUnit").AsDecimal()
                 .WithColumn("ProductFee").AsCurrency()
                 .WithColumn("NatureIndicator").AsString()
                 .WithColumn("Active").AsBoolean().NotNullable();
        }

    }

}
