using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00044, "v00.01.101-Invoice.")]
    public class InitialTables_00044: Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {
            Execute.Sql(string.Format(@"update il set il.LineDeliveryDate = i.InvoiceDeliveryDate,
                        il.LineExchangeRate = i.ExchangeRate
                        from InvoiceLine il
                        inner join Invoice i on il.InvoiceId = i.ID"));

        }
    }
}
