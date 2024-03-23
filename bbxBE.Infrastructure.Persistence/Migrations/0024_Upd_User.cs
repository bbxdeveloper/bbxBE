using bbxBE.Application.BLL;
using bbxBE.Common.Consts;
using FluentMigrator;
using Microsoft.Extensions.Configuration;
using System;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00024, "v00.01.61-add admin user")]
    public class InitialTables_00024 : Migration
    {

        private readonly IConfiguration _configuration;
        public InitialTables_00024(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public override void Down()
        {
        }
        public override void Up()
        {

            var salt = Environment.GetEnvironmentVariable(bbxBEConsts.ENV_PWDSALT);
            var pwdHash = BllAuth.GetPwdHash("admin", salt);

            Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(Name) = 'ADMIN'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active])
                     VALUES
                        ('Adminisztrátor', 'admin@bbxsoftware.com','admin', '{0}', 'Automatikusan létrehozva',1)
               end",
                pwdHash));

        }
    }
}
