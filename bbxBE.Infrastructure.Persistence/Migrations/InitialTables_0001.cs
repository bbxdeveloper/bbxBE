using bbxBE.Infrastructure.Persistence.Contexts;
using Dapper;
using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00001,"v00.00.01")]
    public class InitialTables_00001 : Migration
    {
        public override void Down()
        {
            Delete.Table("USR_USER");
        }
        public override void Up()
        {

            Create.Table("USR_USER")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UPpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("USR_NAME").AsString().NotNullable()
                    .WithColumn("USR_EMAIL").AsString().Nullable()
                    .WithColumn("USR_LOGIN").AsString().NotNullable()
                    .WithColumn("USR_PASSWDHASH").AsString().NotNullable()
                    .WithColumn("USR_COMMENT").AsString().Nullable()
                    .WithColumn("USR_ACTIVE").AsBoolean().WithDefaultValue(true)
                    .WithColumn("DELETED").AsBoolean().WithDefaultValue(false);

        }
    }
}
