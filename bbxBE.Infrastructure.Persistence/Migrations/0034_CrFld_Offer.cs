using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00034, "v00.01.88-Offer new field: UserID")]
    public class InitialTables_00034 : Migration
    {
        public override void Down()
        {
            Delete.Column("UserID").FromTable("Offer");

        }
        public override void Up()
        {
            Alter.Table("Offer")
                .AddColumn("UserID").AsInt64().Nullable().WithDefaultValue(0);
        }
    }
}
