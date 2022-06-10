using bbxBE.Application.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00016, "v00.00.01-Stock")]
    public class InitialTables_00016 : Migration
    {
        public override void Down()
        {
            Delete.Table("Stock");
            Delete.Table("StockCard");
        }
        public override void Up()
        {
            //Készlet
            Create.Table("Stock")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                    .WithColumn("WarehouseID").AsInt64().ForeignKey()
                    .WithColumn("ProductID").AsInt64().ForeignKey()
                    .WithColumn("UserID").AsInt64().ForeignKey()
                    .WithColumn("CalcQty").AsDecimal().NotNullable().WithDefaultValue(0)
                    .WithColumn("RealQty").AsDecimal().NotNullable().WithDefaultValue(0)
                    .WithColumn("AvgCost").AsDecimal().NotNullable().WithDefaultValue(0)        //átlagolt beszerzési egységár
                    .WithColumn("LatestIn").AsDateTime2().Nullable()
                    .WithColumn("LatestOut").AsDateTime2().Nullable();

            //Készletkarton
            Create.Table("StockCard")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                    .WithColumn("WarehouseID").AsInt64().ForeignKey()
                    .WithColumn("ProductID").AsInt64().ForeignKey()
                    .WithColumn("StockID").AsInt64().ForeignKey()
                    .WithColumn("InvoiceID").AsInt64().ForeignKey()
                    .WithColumn("PartnerID").AsInt64().ForeignKey()
                    .WithColumn("UserID").AsInt64().ForeignKey()
                    .WithColumn("Type").AsString().NotNullable().WithDefaultValue(enStockCardTypes.INIT.ToString())
                    .WithColumn("OCalcQty").AsDecimal().NotNullable().WithDefaultValue(0)
                    .WithColumn("ORealQty").AsDecimal().NotNullable().WithDefaultValue(0)
                    .WithColumn("OOutQty").AsDecimal().NotNullable().WithDefaultValue(0)
                    .WithColumn("XCalcQty").AsDecimal().NotNullable().WithDefaultValue(0)
                    .WithColumn("XRealQty").AsDecimal().NotNullable().WithDefaultValue(0)
                    .WithColumn("XOutQty").AsDecimal().NotNullable().WithDefaultValue(0)
                    .WithColumn("NCalcQty").AsDecimal().NotNullable().WithDefaultValue(0)
                    .WithColumn("NRealQty").AsDecimal().NotNullable().WithDefaultValue(0)
                    .WithColumn("NOutQty").AsDecimal().NotNullable().WithDefaultValue(0)
                    .WithColumn("OAvgCost").AsDecimal().NotNullable().WithDefaultValue(0)        //átlagolt beszerzési egységár
                    .WithColumn("NAvgCost").AsDecimal().NotNullable().WithDefaultValue(0)        //átlagolt beszerzési egységár
                    .WithColumn("XRel").AsString().Nullable();                                   //Kapcsolt biyonylat száma, aznosítója, egyéb kapcsolt adatok

        }
    }
}
