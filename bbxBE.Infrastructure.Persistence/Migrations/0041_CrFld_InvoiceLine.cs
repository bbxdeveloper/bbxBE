using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00041, "v00.01.96-InvoiceLine Discount fields")]
    public class InitialTables_00041 : Migration
    {
        public override void Down()
        {
            Delete.Column("LineNetDiscountedAmount").FromTable("InvoiceLine");
            Delete.Column("LineNetDiscountedAmountHUF").FromTable("InvoiceLine");
            Delete.Column("LineVatDiscountedAmount").FromTable("InvoiceLine");
            Delete.Column("LineVatDiscountedAmountHUF").FromTable("InvoiceLine");
            Delete.Column("LineGrossDiscountedAmountNormal").FromTable("InvoiceLine");
            Delete.Column("LineGrossDiscountedAmountNormalHUF").FromTable("InvoiceLine");
        }
        public override void Up()
        {

            Alter.Table("InvoiceLine")
                   .AddColumn("LineNetDiscountedAmount").AsDecimal().WithDefaultValue(0);
            Alter.Table("InvoiceLine")
                   .AddColumn("LineNetDiscountedAmountHUF").AsDecimal().WithDefaultValue(0);
            Alter.Table("InvoiceLine")
                   .AddColumn("LineVatDiscountedAmount").AsDecimal().WithDefaultValue(0);
            Alter.Table("InvoiceLine")
                   .AddColumn("LineVatDiscountedAmountHUF").AsDecimal().WithDefaultValue(0);
            Alter.Table("InvoiceLine")
                   .AddColumn("LineGrossDiscountedAmountNormal").AsDecimal().WithDefaultValue(0);
            Alter.Table("InvoiceLine")
                   .AddColumn("LineGrossDiscountedAmountNormalHUF").AsDecimal().WithDefaultValue(0);
        }
    }
}
