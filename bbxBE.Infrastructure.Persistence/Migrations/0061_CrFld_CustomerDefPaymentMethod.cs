using bbxBE.Common.NAV;
using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00061, "v00.02.08-Customer.DefPaymentMethod")]
    public class InitialTables_00061 : Migration
    {
        public override void Down()
        {
            Delete.Column("DefPaymentMethod").FromTable("Customer");
        }
        public override void Up()
        {


            Alter.Table("Customer")
                .AddColumn("DefPaymentMethod").AsString().NotNullable().WithDefaultValue(PaymentMethodType.CASH.ToString());

            Execute.Sql($"update Customer set DefPaymentMethod = '{PaymentMethodType.CASH.ToString()}'");


        }
    }
}
