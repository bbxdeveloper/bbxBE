using bbxBE.Application.BLL;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using FluentMigrator;
using Microsoft.Extensions.Configuration;
using static bbxBE.Common.NAV.NAV_enums;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00027, "v00.01.64-add users")]
    public class InitialTables_00027: Migration
    {

        private readonly IConfiguration _configuration;
        public InitialTables_00027(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public override void Down()
        {
        }
        public override void Up()
        {

            var salt = _configuration.GetValue<string>(bbxBEConsts.CONF_PwdSalt);

            var pwdHash = BllAuth.GetPwdHash( "mj", salt);
            
            Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'MJ'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active])
                     VALUES
                        ('Mezei József', 'mezeirelaxvill@gmail.com','mj', '{0}', 'Automatikusan létrehozva',1)
               end", 
                pwdHash));
            
            pwdHash = BllAuth.GetPwdHash("ri", salt);
            Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'RI'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active])
                     VALUES
                        ('Rékasi István', 'rekasi@relaxvill.hu','ri', '{0}', 'Automatikusan létrehozva',1)
               end",
                pwdHash));

            pwdHash = BllAuth.GetPwdHash("kk", salt);
            Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'KK'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active])
                     VALUES
                        ('Kormos Krisztián', 'vevoszolgalat@relaxvill.hu','kk', '{0}', 'Automatikusan létrehozva',1)
               end",
                pwdHash));

            pwdHash = BllAuth.GetPwdHash("mp", salt);
            Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'MP'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active])
                     VALUES
                        ('Máté Péter', 'ajanlat@relaxvill.hu','mp', '{0}', 'Automatikusan létrehozva',1)
               end",
                pwdHash));

            pwdHash = BllAuth.GetPwdHash("ti", salt);
            Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'TI'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active])
                     VALUES
                        ('Török István', 'kecskemet@relaxvill.hu','ti', '{0}', 'Automatikusan létrehozva',1)
               end",
                pwdHash));
        }
    }
}
