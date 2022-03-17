using bbxBE.Application.Enums;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00010, "v00.00.01")]
    public class InitialTables_00010 : Migration
    {
        public override void Down()
        {
            Delete.Index("INX_InvoiceNumber")
                   .OnTable("Invoice");
            Delete.Index("INX_InvoiceIssueDate")
                        .OnTable("Invoice");
            Delete.Index("INX_InvoiceDeliveryDate")
                        .OnTable("Invoice");
            Delete.Table("Invoice");

            Delete.Index("INX_InvoiceOrdernumber")
                        .OnTable("InvoiceOrdernumber");
            Delete.Table("InvoiceOrdernumber");


            Delete.Table("AdditionalInvoiceData");

            Delete.Table("SummaryByVatRate");
        }
        public override void Up()
        {

            Create.Table("Invoice")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)

                    //InvoiceData 
                    .WithColumn("InvoiceNumber").AsString().NotNullable()
                    .WithColumn("InvoiceIssueDate").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("CompletenessIndicator").AsBoolean().NotNullable().WithDefaultValue(false)

                    //invoiceHead
                    .WithColumn("SupplierID").AsInt64().NotNullable().ForeignKey()
                    .WithColumn("CustomerID").AsInt64().NotNullable().ForeignKey()
                    .WithColumn("InvoiceCategory").AsString().NotNullable().WithDefaultValue(InvoiceCategoryType.NORMAL.ToString())
                    .WithColumn("InvoiceDeliveryDate").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("PaymentDate").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("CurrencyCode").AsString().NotNullable().WithDefaultValue(enCurrencyCodes.HUF.ToString())
                    .WithColumn("ExchangeRate").AsDecimal().NotNullable().WithDefaultValue(1)
                    .WithColumn("UtilitySettlementIndicator").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("InvoiceAppearance").AsString().NotNullable().WithDefaultValue(InvoiceCategoryType.NORMAL.ToString())

                    //invoiceSummary
                    .WithColumn("InvoiceNetAmount").AsCurrency().NotNullable()
                    .WithColumn("InvoiceNetAmountHUF").AsCurrency().NotNullable()
                    .WithColumn("InvoiceVatAmount").AsCurrency().NotNullable()
                    .WithColumn("InvoiceVatAmountHUF").AsCurrency().NotNullable()
                    .WithColumn("InvoiceGrossAmount").AsCurrency().NotNullable()
                    .WithColumn("invoiceGrossAmountHUF").AsCurrency().NotNullable();


            Create.Index("INX_InvoiceNumber")
                         .OnTable("Invoice")
                         .OnColumn("InvoiceNumber").Ascending()
                         .WithOptions().Clustered();

            Create.Index("INX_InvoiceIssueDate")
                         .OnTable("Invoice")
                         .OnColumn("InvoiceIssueDate").Ascending()
                         .WithOptions().NonClustered();

            Create.Index("INX_InvoiceDeliveryDate")
                         .OnTable("Invoice")
                         .OnColumn("InvoiceDeliveryDate").Ascending()
                         .WithOptions().NonClustered();



            Create.Table("InvoiceOrdernumber")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                    .WithColumn("InvoiceID").AsInt64().NotNullable().ForeignKey()
                    .WithColumn("OrderNumber").AsString().NotNullable();

            Create.Index("INX_InvoiceOrdernumber")
                         .OnTable("InvoiceOrdernumber")
                         .OnColumn("OrderNumber").Ascending()
                         .WithOptions().NonClustered();

            Create.Table("AdditionalInvoiceData")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                    .WithColumn("InvoiceID").AsInt64().NotNullable().ForeignKey()
                    .WithColumn("DataName").AsString().NotNullable()
                    .WithColumn("DataDescription").AsString().Nullable()
                    .WithColumn("DataValue").AsString().Nullable();




            Create.Table("SummaryByVatRate")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                    .WithColumn("InvoiceID").AsInt64().NotNullable().ForeignKey()
                    .WithColumn("VatRateID").AsInt64().NotNullable().ForeignKey()
                    .WithColumn("VatRateNetAmount").AsCurrency().NotNullable()
                    .WithColumn("VatRateNetAmountHUF").AsCurrency().NotNullable();


            Create.Table("SummaryByVatRate")
                     .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                     .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                     .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                     .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                     .WithColumn("InvoiceID").AsInt64().NotNullable().ForeignKey()
                     .WithColumn("LineNumber").AsInt16().NotNullable()
                    .WithColumn("LineExpressionIndicator").AsBoolean().WithDefaultValue(true)           // Ha a tag értéke true, akkor adott számlasorban kötelező megadni:
                                                                                                        // a.a termék vagy szolgáltatás nevét
                                                                                                        // b.mennyiségét
                                                                                                        // c.mennyiségi egységét
                                                                                                        // d.egységárát

                    .WithColumn("LineNatureIndicator").AsString().NotNullable()
                    .WithColumn("LineDescription").AsString().NotNullable()



        }

    }

}
