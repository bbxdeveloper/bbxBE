using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00057, "v00.02.08 -InvCtl indexes")]
    public class InitialTables_00057 : Migration
    {
        public override void Down()
        {
            Delete.Index("INX_InvCtrlInvCtrlPeriodID")
            .OnTable("InvCtrl");
        }
        public override void Up()
        {
            Create.Index("INX_InvCtrlInvCtrlPeriodID")
                         .OnTable("InvCtrl")
                         .OnColumn("InvCtlPeriodID").Ascending()
                         .OnColumn("InvCtrlDate").Ascending()
                         .WithOptions().NonClustered();
        }
    }
}
