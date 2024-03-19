using bbxBE.Common.Enums;
using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00091, "v00.02.56-POrder tábla")]
    public class InitialTables_00091 : Migration
    {
        public override void Down()
        {
            Delete.Table("POrderLine");
            Delete.Table("POrder");
        }
        public override void Up()
        {
            Create.Table("POrder")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)

                    .WithColumn("WarehouseID").AsInt64().NotNullable().ForeignKey()
                    .WithColumn("SupplierID").AsInt64().NotNullable().ForeignKey()
                    .WithColumn("CustomerID").AsInt64().NotNullable().ForeignKey()
                    .WithColumn("POrderNumber").AsString()
                    .WithColumn("POrderType").AsString()
                    .WithColumn("POrderIssueDate").AsDateTime2()
                    .WithColumn("CurrencyCode").AsString().NotNullable().WithDefaultValue(enCurrencyCodes.HUF.ToString())
                    .WithColumn("ExchangeRate").AsDecimal().NotNullable().WithDefaultValue(1)
                    .WithColumn("Comment").AsString().Nullable()
                    .WithColumn("UserID").AsInt64().NotNullable().ForeignKey();

            Create.Table("POrderLine")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)

                    .WithColumn("POrderID").AsInt64().NotNullable().ForeignKey()
                    .WithColumn("ProductID").AsInt64().Nullable().ForeignKey()                         //Opcionális
                    .WithColumn("ProductCode").AsString().Nullable()                                   //Opcionális!
                    .WithColumn("LineDescription").AsString().NotNullable()
                    .WithColumn("Quantity").AsDecimal().Nullable()
                    .WithColumn("UnitOfMeasure").AsString().NotNullable().WithDefaultValue("")
                    .WithColumn("VatRateID").AsInt64().NotNullable().ForeignKey()
                    .WithColumn("VatPercentage").AsDecimal().Nullable()
                    .WithColumn("UnitPrice").AsCurrency().NotNullable().WithDefaultValue(0)
                    .WithColumn("UnitPriceHUF").AsCurrency().NotNullable().WithDefaultValue(0)
                    .WithColumn("UnitVat").AsCurrency().NotNullable().WithDefaultValue(0)
                    .WithColumn("UnitVatHUF").AsCurrency().NotNullable().WithDefaultValue(0)
                    .WithColumn("UnitGross").AsCurrency().NotNullable().WithDefaultValue(0)
                    .WithColumn("UnitGrossHUF").AsCurrency().NotNullable().WithDefaultValue(0);
        }
    }
}
