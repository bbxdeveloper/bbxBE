using bbxBE.Common.Enums;
using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00082, "v00.02.41-InvPayment, kiegyenlítések tábla")]
    public class InitialTables_00082 : Migration
    {
        public override void Down()
        {
            Delete.Table("InvPayment");
        }
        public override void Up()
        {
            Create.Table("InvPayment")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)

                    .WithColumn("InvoiceID").AsInt64()
                    .WithColumn("BankTransaction").AsString().NotNullable()
                    .WithColumn("InvPaymentDate").AsDateTime2().NotNullable()
                    .WithColumn("InvPaymentAmount").AsDecimal().NotNullable()
                    .WithColumn("CurrencyCode").AsString().NotNullable().WithDefaultValue(enCurrencyCodes.HUF.ToString())
                    .WithColumn("ExchangeRate").AsDecimal().NotNullable()
                    .WithColumn("InvPaymentAmountHUF").AsDecimal().NotNullable()
                    .WithColumn("UserID").AsInt64().NotNullable().WithDefaultValue(0);

            Create.Index("INX_InvPaymentInvoiceID")
                         .OnTable("InvPayment")
                         .OnColumn("InvoiceID").Ascending()
                         .WithOptions().NonClustered();
            Create.Index("INX_InvPaymentBankTransaction")
                         .OnTable("InvPayment")
                         .OnColumn("BankTransaction").Ascending()
                         .WithOptions().NonClustered();

        }
    }
}
