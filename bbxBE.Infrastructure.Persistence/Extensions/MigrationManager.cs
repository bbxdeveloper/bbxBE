﻿using bbxBE.Infrastructure.Persistence.Migrations;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;

public static class MigrationManager
{
    public static IHost MigrateDatabase(this IHost host)
    {
        //https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/
        using (var scope = host.Services.CreateScope())
        {
            var databaseService = scope.ServiceProvider.GetRequiredService<DatabaseMigration>();
            var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

            try
            {
                IDbConnection connection = new SqlConnection(databaseService.Context.ConnectionString);
                var dbName = connection.Database;
                databaseService.CreateDatabase(dbName);
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
