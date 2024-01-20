using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00083, "v00.02.41-InvPayment: PayableAmount, PayableAmountHUF mezők")]
    public class InitialTables_00083 : Migration
    {
        public override void Down()
        {
            Delete.Column("InvPayment").FromTable("PayableAmount");
            Delete.Column("InvPayment").FromTable("PayableAmountHUF");
        }
        public override void Up()
        {
            Alter.Table("InvPayment")
                    .AddColumn("PayableAmount").AsDecimal().WithDefaultValue(0)
                    .AddColumn("PayableAmountHUF").AsDecimal().WithDefaultValue(0);




        }
    }
}
