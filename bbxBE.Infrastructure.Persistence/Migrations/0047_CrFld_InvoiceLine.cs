using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00047, "v00.02.08-InvoiceLine.Lind")]
    public class InitialTables_00047: Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {

            //Sorra adott kedvezmény.
            //értéke a számlára adott kedvezmény, vagy a gyűjtőszámlán, a kapcsolt tételhet adott kedvezmény
            //
            Alter.Table("InvoiceLine")
                .AddColumn("LineDiscountPercent").AsDecimal().WithDefaultValue(0);

            Execute.Sql(string.Format(@"update il
                            set LineDiscountPercent  = case when dn.ID is null then inv.InvoiceDiscountPercent else dn.InvoiceDiscountPercent end
                            from InvoiceLine il
                            inner join Invoice inv on inv.ID = il.InvoiceID
                            left outer join Invoice dn on dn.ID = il.RelDeliveryNoteInvoiceID
                            "));

        }
    }
}
