using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00043, "v00.01.100-Invoice.WorkNumber, PriceReview")]
    public class InitialTables_00043: Migration
    {
        public override void Down()
        {
            Delete.Column("WorkNumber").FromTable("Invoice");
            Delete.Column("PriceReview").FromTable("InvoiceLine");
        }
        public override void Up()
        {


            Alter.Table("Invoice")
                .AddColumn("WorkNumber").AsString().Nullable();
            Alter.Table("InvoiceLine")
                .AddColumn("PriceReview").AsBoolean().Nullable().WithDefaultValue(false);

        }
    }
}
