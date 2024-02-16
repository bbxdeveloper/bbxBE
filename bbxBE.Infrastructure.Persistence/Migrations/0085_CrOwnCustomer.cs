using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00085, "v00.02.50-OWN customer létrehozása, amennyiben nincs")]
    public class InitialTables_0085 : Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {
            var sql = @"if not exists( select * from Customer where IsOwnData =1)
INSERT INTO [dbo].[Customer]
           ([CustomerName]
           ,[CustomerBankAccountNumber]
           ,[CustomerVatStatus]
           ,[TaxpayerId]
           ,[VatCode]
           ,[CountyCode]
           ,[ThirdStateTaxId]
           ,[CountryCode]
           ,[PostalCode]
           ,[City]
           ,[AdditionalAddressDetail]
           ,[Comment]
           ,[IsOwnData])
     VALUES
           ('Relax Kft. TESZT'
		   ,'10404993-50515755-74861005'
		   ,'DOMESTIC'
		   ,'10301584'
		   ,'2'
		   ,'43'
		   ,''
		   ,'HU'
		   ,'1222'
		   ,'Budapest'
		   ,'Háros u. 47-49'
		   ,''
		   ,1)";
            Execute.Sql(sql);

        }
    }
}
