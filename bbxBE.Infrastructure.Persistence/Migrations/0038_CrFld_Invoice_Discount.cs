using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(0038, "v00.01.94-Invoice - DiscountPercent, Discount, Discount HUF")]
    public class InitialTables_00038 : Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {



            Alter.Table("Invoice")
                .AddColumn("InvoiceDiscountPercent").AsDecimal().WithDefaultValue(0);
            Alter.Table("Invoice")
                .AddColumn("InvoiceDiscount").AsDecimal().WithDefaultValue(0);
            Alter.Table("Invoice")
                .AddColumn("InvoiceDiscountHUF").AsDecimal().WithDefaultValue(0);

        }
    }
}
