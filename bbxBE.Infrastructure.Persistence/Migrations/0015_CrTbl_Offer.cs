using bbxBE.Application.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00015, "v00.00.01")]
    public class InitialTables_00015 : Migration
    {
        public override void Down()
        {
            Delete.Table("Offer");
            Delete.Table("OfferLine");

            /*
                delete VersionInfo where Version = 15
                drop table Offer
                drop table OfferLine
             */
        }
        public override void Up()
        {

            Create.Table("Offer")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)

                    .WithColumn("OfferNumber").AsString().NotNullable()
                    .WithColumn("OfferIssueDate").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("OfferVaidityDate").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("CustomerID").AsInt64().NotNullable().ForeignKey()
                    .WithColumn("Copies").AsInt16().Nullable()
                    .WithColumn("Notice").AsString(int.MaxValue).NotNullable()
                    .WithColumn("CurrencyCode").AsString().NotNullable().WithDefaultValue(enCurrencyCodes.HUF.ToString())
                    .WithColumn("ExchangeRate").AsDecimal().NotNullable().WithDefaultValue(1);


            Create.Index("INX_OfferNumber")
                         .OnTable("Offer")
                         .OnColumn("OfferNumber").Ascending()
                         .WithOptions().NonClustered();

            Create.Index("INX_OfferIssueDate")
                         .OnTable("Offer")
                         .OnColumn("OfferIssueDate").Ascending()
                         .WithOptions().NonClustered();

            Create.Index("INX_OfferVaidityDate")
                         .OnTable("Offer")
                         .OnColumn("OfferVaidityDate").Ascending()
                         .WithOptions().NonClustered();



            Create.Table("OfferLine")
                     .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                     .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                     .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                     .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)

                     .WithColumn("OfferID").AsInt64().NotNullable().ForeignKey()
                     .WithColumn("LineNumber").AsInt16().NotNullable()
                     .WithColumn("ProductID").AsInt64().Nullable().ForeignKey()                         //Opcionális
                     .WithColumn("ProductCode").AsString().Nullable()                                   //Opcionális!
                     .WithColumn("LineDescription").AsString().NotNullable()
                     .WithColumn("VatRateID").AsInt64().NotNullable().ForeignKey()
                     .WithColumn("VatPercentage").AsDecimal().Nullable()
                     .WithColumn("UnitOfMeasure").AsString().NotNullable().WithDefaultValue("")
                     .WithColumn("UnitPrice").AsCurrency().NotNullable().WithDefaultValue(0)
                     .WithColumn("UnitPriceHUF").AsCurrency().NotNullable().WithDefaultValue(0)
                     .WithColumn("UnitVat").AsCurrency().NotNullable().WithDefaultValue(0)
                     .WithColumn("UnitVatHUF").AsCurrency().NotNullable().WithDefaultValue(0)
                     .WithColumn("UnitGross").AsCurrency().NotNullable().WithDefaultValue(0)
                     .WithColumn("UnitGrossHUF").AsCurrency().NotNullable().WithDefaultValue(0);

            Create.Index("INX_OfferLineProduct")
                         .OnTable("OfferLine")
                         .OnColumn("ProductID").Ascending()
                         .WithOptions().NonClustered();

            Execute.Sql(string.Format(@"
                if not exists (select * from Counter where CounterCode='{0}')
                begin
                    insert into Counter ([WarehouseID],[CounterCode],[CounterDescription],[Prefix],[CurrentNumber],[NumbepartLength],[Suffix],[CounterPool])
                    select ID, '{2}','{3}', '{4}', {5}, {6}, '{7}', '{8}' from Warehouse where WarehouseCode='{1}'
               end",
                bbxBEConsts.DEF_OFFERCOUNTER,
                bbxBEConsts.DEF_WAREHOUSE, bbxBEConsts.DEF_OFFERCOUNTER, "Árajánlatok", bbxBEConsts.DEF_OFFERCOUNTER, 1, 5, "/", ""));

        }
    }
}
