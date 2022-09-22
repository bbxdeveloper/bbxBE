using bbxBE.Common.Consts;
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
    [Migration(00023,"v00.01.61 USR_USER -> User")]
    public class CrTbl_0001_User: Migration
    {
        public override void Down()
        {
            Delete.Table("Users");
        }
        public override void Up()
        {

            Create.Table("Users")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                    .WithColumn("Name").AsString().NotNullable()
                    .WithColumn("Email").AsString().Nullable()
                    .WithColumn("LoginName").AsString().NotNullable()
                    .WithColumn("PasswordHash").AsString().NotNullable()
                    .WithColumn("Comment").AsString().Nullable()
                    .WithColumn("Active").AsBoolean().WithDefaultValue(true);

            Execute.Sql( @"
                insert Users (Deleted, Name, Email, LoginName, PasswordHash, Comment, Active) 
                select [Deleted],[USR_NAME],[USR_EMAIL],[USR_LOGIN],[USR_PASSWDHASH],[USR_COMMENT],[USR_ACTIVE] FROM [dbo].[USR_USER]
                ");


            Delete.Table("USR_USER");


        }
    }
}
