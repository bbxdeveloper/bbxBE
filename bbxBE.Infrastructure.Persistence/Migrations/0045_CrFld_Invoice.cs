using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00045, "v00.01.101-InvoiceLine.NoDiscount")]
    public class InitialTables_00045: Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {
            Alter.Table("InvoiceLine")
                .AddColumn("NoDiscount").AsBoolean().NotNullable().WithDefaultValue(false);
   
            Execute.Sql(string.Format(@"update il 
                set il.NoDiscount = pr.NoDiscount
                from InvoiceLine il
                inner join Product pr on pr.ID = il.ProductID"));

        }
    }
}
