using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00060, "v00.02.08 -WhsTransfer TransferDateIn")]
    public class InitialTables_00060 : Migration
    {
        public override void Down()
        {
            Delete.Column("TransferDateIn").FromTable("WhsTransfer");
        }
        public override void Up()
        {
            Alter.Table("WhsTransfer")
                .AddColumn("TransferDateIn").AsDateTime2().Nullable();

            Execute.Sql(string.Format(@"update WhsTransfer set TransferDateIn = TransferDate where WhsTransferStatus = 'COMPLETED'"));

        }
    }
}
