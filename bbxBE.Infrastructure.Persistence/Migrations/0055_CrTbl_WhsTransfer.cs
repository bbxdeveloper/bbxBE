﻿using bbxBE.Common.Consts;
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
    [Migration(00055,"v00.02.08 WhsTransfer - raktárközi átadás")]
    public class InitialTables_00055: Migration
    {
        public override void Down()
        {
            Delete.Table("WhsTransfer");
            Delete.Table("WhsTransferLine");
        }
        public override void Up()
        {

            Create.Table("WhsTransfer")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                    .WithColumn("WhsTransferNumber").AsString().NotNullable()
                    .WithColumn("FromWarehouseID").AsInt64().ForeignKey()
                    .WithColumn("ToWarehouseID").AsInt64().ForeignKey()
                    .WithColumn("TransferDate").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UserID").AsInt64().ForeignKey();

            Create.Index("INX_WhsTransferNumber")
             .OnTable("WhsTransfer")
             .OnColumn("WhsTransferNumber").Ascending()
             .WithOptions().NonClustered();

            Create.Index("INX_FromWarehouseID")
             .OnTable("WhsTransfer")
             .OnColumn("FromWarehouseID").Ascending()
             .OnColumn("TransferDate").Ascending()
             .WithOptions().NonClustered();

            Create.Index("INX_ToWarehouseID")
             .OnTable("WhsTransfer")
             .OnColumn("ToWarehouseID").Ascending()
             .OnColumn("TransferDate").Ascending()
             .WithOptions().NonClustered();

            Create.Table("WhsTransferLine")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)
                    .WithColumn("WhsTransferID").AsInt64().ForeignKey()
                    .WithColumn("WhsTransferLineNumber").AsInt16().NotNullable()
                    .WithColumn("ProductID").AsInt64().ForeignKey()
                    .WithColumn("Quantity").AsDecimal().NotNullable().WithDefaultValue(0)
                    .WithColumn("UnitOfMeasure").AsString().NotNullable().WithDefaultValue("")
                    .WithColumn("UnitPrice").AsCurrency().NotNullable().WithDefaultValue(0)
                    .WithColumn("AvgCost").AsDecimal().NotNullable().WithDefaultValue(0);        //átlagolt beszerzési egységár

            Create.Index("INX_WhsTransferLineWarehouseID")
             .OnTable("WhsTransferLine")
             .OnColumn("WhsTransferID").Ascending()
             .OnColumn("WhsTransferLineNumber").Ascending()
             .WithOptions().NonClustered();
        }
    }
}
