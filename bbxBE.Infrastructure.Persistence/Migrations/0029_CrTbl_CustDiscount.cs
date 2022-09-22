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
    [Migration(00029,"v00.01.65 CustDiscount")]
    public class InitialTables_00029: Migration
    {
        public override void Down()
        {
            Delete.Table("CustDiscount");
        }
        public override void Up()
        {

            Create.Table("CustDiscount")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                    .WithColumn("CustomerID").AsInt64().ForeignKey()
                    .WithColumn("ProductGroupID").AsInt64().ForeignKey()
                    .WithColumn("Discount").AsDecimal().NotNullable().WithDefaultValue(0)
;

            Create.Index("INX_CustDiscount")
                         .OnTable("CustDiscount")
                         .OnColumn("CustomerID").Ascending()
                         .OnColumn("ProductGroupID").Ascending()
                         .WithOptions().NonClustered();
        }
    }
}
