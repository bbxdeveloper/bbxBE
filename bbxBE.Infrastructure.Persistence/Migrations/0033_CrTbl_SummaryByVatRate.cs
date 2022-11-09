using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00033, "v00.01.84-SummaryByVatRate")]
    public class InitialTables_00033 : Migration
    {
        public override void Down()
        {
            /*
                delete VersionInfo where Version = 33
             */
        }
        public override void Up()
        {
            Alter.Table("SummaryByVatRate")
                .AddColumn("VatRateVatAmount").AsCurrency().NotNullable().WithDefaultValue(0);
            Alter.Table("SummaryByVatRate")
                .AddColumn("VatRateVatAmountHUF").AsCurrency().NotNullable().WithDefaultValue(0);
            Alter.Table("SummaryByVatRate")
                .AddColumn("VatRateGrossAmount").AsCurrency().NotNullable().WithDefaultValue(0);
            Alter.Table("SummaryByVatRate")
                .AddColumn("VatRateGrossAmountHUF").AsCurrency().NotNullable().WithDefaultValue(0);


            Execute.Sql(string.Format(@"
                 UPDATE [dbo].[SummaryByVatRate] set VatRateVatAmount = VatRateNetAmount,  VatRateVatAmountHUF = VatRateNetAmountHUF
                 "));
            Execute.Sql(string.Format(@"
                 UPDATE [dbo].[SummaryByVatRate] set VatRateNetAmount = VatNetAmount,  VatRateNetAmountHUF = VatNetAmountHUF
                 "));
            Execute.Sql(string.Format(@"
                 UPDATE [dbo].[SummaryByVatRate] set VatRateGrossAmount = ROUND( VatRateNetAmount+VatRateNetAmount,1),  VatRateGrossAmountHUF = ROUND( VatRateNetAmountHUF+VatRateNetAmountHUF,1)
                 "));

            Delete.Column("VatNetAmount").FromTable("SummaryByVatRate");
            Delete.Column("VatNetAmountHUF").FromTable("SummaryByVatRate");

            //elírások javítása
            Rename.Column("lineVatAmount").OnTable("InvoiceLine").To("LineVatAmount");
            Rename.Column("lineVatAmountHUF").OnTable("InvoiceLine").To("LineVatAmountHUF");
            Rename.Column("lineGrossAmountNormal").OnTable("InvoiceLine").To("LineGrossAmountNormal");
            Rename.Column("lineGrossAmountNormalHUF").OnTable("InvoiceLine").To("LineGrossAmountNormalHUF");

            Rename.Column("invoiceGrossAmountHUF").OnTable("Invoice").To("InvoiceGrossAmountHUF");
        }
    }
}
