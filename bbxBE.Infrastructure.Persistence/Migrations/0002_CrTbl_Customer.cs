using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00002, "v00.00.01")]
    public class InitialTables_00002 : Migration
    {
        public override void Down()
        {
            Delete.Table("Customer");
        }
        public override void Up()
        {

            Create.Table("Customer")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                    .WithColumn("CustomerName").AsString().NotNullable()
                    .WithColumn("CustomerBankAccountNumber").AsString()
                    .WithColumn("CustomerVatStatus").AsString().NotNullable()
                    .WithColumn("TaxpayerId").AsString().Nullable()
                    .WithColumn("VatCode").AsString().Nullable()
                    .WithColumn("CountyCode").AsString().Nullable()
                    .WithColumn("ThirdStateTaxId").AsString().Nullable()
                    .WithColumn("CountryCode").AsString().NotNullable()
                    .WithColumn("Region").AsString().Nullable()
                    .WithColumn("PostalCode").AsString().Nullable()
                    .WithColumn("City").AsString().NotNullable()
                    .WithColumn("AdditionalAddressDetail").AsString().NotNullable()
                    .WithColumn("Comment").AsString().Nullable();
        }

    }

}
