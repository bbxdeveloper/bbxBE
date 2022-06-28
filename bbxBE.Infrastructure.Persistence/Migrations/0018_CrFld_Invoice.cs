using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00018, "v00.00.01-Invoice.UserID")]
    public class InitialTables_00018 : Migration
    {
        public override void Down()
        {
            Delete.Column("UserID").FromTable("Invoice");
        }
        public override void Up()
        {


            Alter.Table("Invoice")
                .AddColumn("UserID").AsInt64().Nullable().WithDefaultValue(0);

            /*
            Update.Table("Invoice").Set(new { UserID = 0 }).AllRows();
            Alter.Table("Invoice")
                 .AddColumn("UserID").AsInt64().NotNullable().WithDefaultValue(0);
            */
        }
    }
}
