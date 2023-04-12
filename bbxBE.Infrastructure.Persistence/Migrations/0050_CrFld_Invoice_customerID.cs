using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00050, "v00.02.08-InvoiceLine. CustomerID is nullable")]
    public class InitialTables_00050: Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {

            //Sorra adott kedvezmény.
            //értéke a számlára adott kedvezmény, vagy a gyűjtőszámlán, a kapcsolt tételhet adott kedvezmény
            //
            Alter.Table("Invoice")
                .AlterColumn("CustomerID").AsInt64().Nullable();
        }
    }
}
