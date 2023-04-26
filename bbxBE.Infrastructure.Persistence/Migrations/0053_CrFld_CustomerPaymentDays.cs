using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00053, "v00.02.08-Customer.PaymentDays")]
    public class InitialTables_00053 : Migration
    {
        public override void Down()
        {
            Delete.Column("PaymentDays").FromTable("Customer");
        }
        public override void Up()
        {


            Alter.Table("Customer")
                .AddColumn("PaymentDays").AsInt16().NotNullable().WithDefaultValue(8);

            Execute.Sql(string.Format(@"update Customer set PaymentDays = 8"));


        }
    }
}
