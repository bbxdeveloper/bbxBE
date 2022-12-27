using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(0039, "v00.01.95-Offer - IsBrutto")]
    public class InitialTables_00039 : Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {



            Alter.Table("Offer")
                .AddColumn("IsBrutto").AsBoolean().WithDefaultValue(false);

        }
    }
}
