using bbxBE.Application.Consts;
using bbxBE.Infrastructure.Persistence.Migrations;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

public static class MigrationManager
{
    public static IHost MigrateDatabase(this IHost host)
    {
        //https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/
        using (var scope = host.Services.CreateScope())
        {
            var databaseService = scope.ServiceProvider.GetRequiredService<Database>();
            var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

            try
            {
                databaseService.CreateDatabase(bbxBEConsts.DB);
                migrationService.ListMigrations();
                migrationService.MigrateUp();
            }
            catch
            {
                throw;
            }
        }

        return host;
    }
}
