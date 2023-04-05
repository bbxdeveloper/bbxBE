using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00048, "v00.02.08-Customer.UnitPrice")]
    public class InitialTables_00048 : Migration
    {
        public override void Down()
        {
            Delete.Column("UnitPrice").FromTable("Customer");
        }
        public override void Up()
        {


            Alter.Table("Customer")
                .AddColumn("UnitPrice").AsString().Nullable();

            Execute.Sql(string.Format(@"update Customer set UnitPrice = '{0}'", enUnitPrice.LIST));


        }
    }
}
