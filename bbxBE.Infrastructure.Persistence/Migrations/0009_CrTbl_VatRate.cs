using bbxBE.Common.Consts;
using bbxBE.Domain.Entities;
using FluentMigrator;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00009, "v00.00.01-VatRate")]
    public class InitialTables_00009 : Migration
    {
        public override void Down()
        {
            Delete.Table("VatRate");
        }
        public override void Up()
        {


            Create.Table("VatRate")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                    .WithColumn("VatRateCode").AsString().NotNullable().Indexed()
                    .WithColumn("VatPercentage").AsDecimal().Nullable()
                    .WithColumn("VatContent").AsDecimal().Nullable()
                    .WithColumn("VatExemptionCase").AsString().Nullable()
                    .WithColumn("VatExemptionReason").AsString().Nullable()
                    .WithColumn("VatOutOfScopeCase").AsString().Nullable()
                    .WithColumn("VatOutOfScopeReason").AsString().Nullable()
                    .WithColumn("VatDomesticReverseCharge").AsBoolean().WithDefaultValue(false)
                    .WithColumn("MarginSchemeIndicator").AsString().Nullable()
                    .WithColumn("vatAmountMismatchVatRate").AsDecimal().Nullable()
                    .WithColumn("vatAmountMismatchCase").AsString().Nullable()
                    .WithColumn("NoVatCharge").AsBoolean().WithDefaultValue(false);

            Insert.IntoTable("VatRate").Row(new { VatRateCode = bbxBEConsts.VATCODE_27, VatPercentage = 0.27 });
            Insert.IntoTable("VatRate").Row(new { VatRateCode = bbxBEConsts.VATCODE_KBAET, VatPercentage = 0, VatExemptionCase = "KBAET", VatExemptionReason = "Áfa tv. 89. §" });
            Insert.IntoTable("VatRate").Row(new { VatRateCode = bbxBEConsts.VATCODE_FA, VatPercentage = 0, VatDomesticReverseCharge = true });
            /*
                        Insert.IntoTable("VatRate").Row(new { VatRateCode = "5%", VatPercentage = 0.05 });
                        Insert.IntoTable("VatRate").Row(new { VatRateCode = "TAM", VatPercentage = 0, VatExemptionCase = "TAM", VatExemptionReason = "Mentes ÁFA tv. 85.§ (1) i)" });   //Tárgyi adómentes
                        Insert.IntoTable("VatRate").Row(new { VatRateCode = "AAM", VatPercentage = 0, VatExemptionCase = "AAM", VatExemptionReason = "Alanyi adómentes" });         //alanyi adómentes
                        Insert.IntoTable("VatRate").Row(new { VatRateCode = "ATK", VatPercentage = 0, VatOutOfScopeCase = "ATK", VatOutOfScopeReason = "ÁFA hatályán kívül" });
            */

        }

    }

}
