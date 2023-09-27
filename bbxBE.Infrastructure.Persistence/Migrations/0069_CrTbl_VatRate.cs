using bbxBE.Common.Consts;
using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00069, "v00.02.27 -VatRate bővítés ")]
    public class InitialTables_00069 : Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {

            Alter.Table("VatRate")
                .AddColumn("VatRateName").AsString().Nullable();

            Execute.Sql(string.Format(@"update VatRate set VatRateName = '27% áfa' where VatRateCode = '{0}'", bbxBEConsts.VATCODE_27));
            Execute.Sql(string.Format(@"update VatRate set VatRateName = 'KBAET adómentes Közösségen belüli termékértékesítés, új közlekedési eszköz nélkül' where VatRateCode = '{0}'", bbxBEConsts.VATCODE_KBAET));
            Execute.Sql(string.Format(@"update VatRate set VatRateName = 'Fordított adózás' where VatRateCode = '{0}'", bbxBEConsts.VATCODE_FA));

            Insert.IntoTable("VatRate").Row(new
            {
                VatRateCode = bbxBEConsts.VATCODE_TAM,
                VatRateName = "tárgyi adómentes ill. a tevékenység közérdekű vagy speciális jellegére tekintettel adóme",
                VatPercentage = 0,
                VatExemptionCase = bbxBEConsts.VATCODE_TAM,
                VatExemptionReason = bbxBEConsts.DEF_VATREASON_TAM
            });   //Tárgyi adómentes

        }

    }

}
