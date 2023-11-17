using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00067, "v00.02.23-NAV táblák")]
    public class InitialTables_00067 : Migration
    {
        public override void Down()
        {
            Delete.Table("NAVXChange");
            Delete.Table("NAVXChange");
        }
        public override void Up()
        {
            Create.Table("NAVXChange")
                    .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)

                    .WithColumn("InvoiceID").AsInt64()
                    .WithColumn("InvoiceNumber").AsString()
                    .WithColumn("InvoiceXml").AsString(int.MaxValue)
                    .WithColumn("Status").AsString().NotNullable()                               //CREATED-SENT-DONE/ABORTED
                    .WithColumn("Notice").AsString(int.MaxValue)
                    .WithColumn("Operation").AsString().NotNullable()                            //CREATE/MODIFY

                    .WithColumn("TokenTime").AsDateTime2()
                    .WithColumn("TokenRequest").AsString(int.MaxValue)
                    .WithColumn("Token").AsString()
                    .WithColumn("TokenResponse").AsString(int.MaxValue)
                    .WithColumn("TokenFuncCode").AsString()                                     //OK,WARN,NULLTOKEN,EMPTYTOKEN
                    .WithColumn("TokenMessage").AsString(int.MaxValue)


                    .WithColumn("SendTime").AsDateTime2()
                    .WithColumn("SendRequest").AsString(int.MaxValue)
                    .WithColumn("SendResponse").AsString(int.MaxValue)
                    .WithColumn("SendFuncCode").AsString()                                     //OK,ERROR,POSTERROR
                    .WithColumn("SendMessage").AsString(int.MaxValue)

                    .WithColumn("QueryTime").AsDateTime2()
                    .WithColumn("QueryRequest").AsString(int.MaxValue)
                    .WithColumn("QueryResponse").AsString(int.MaxValue)
                    .WithColumn("QueryFuncCode").AsString()                                     //OK,ERROR,POSTERROR
                    .WithColumn("QueryMessage").AsString(int.MaxValue)

                    .WithColumn("TransactionID").AsString();

            Create.Index("INX_NAVXChangeTransactionID")
                         .OnTable("NAVXChange")
                         .OnColumn("TransactionID").Ascending()
                         .WithOptions().NonClustered();

            Create.Table("NAVXResult")
                           .WithColumn("ID").AsInt64().NotNullable().PrimaryKey().Identity()
                           .WithColumn("CreateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                           .WithColumn("UpdateTime").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                           .WithColumn("Deleted").AsBoolean().WithDefaultValue(false)

                    .WithColumn("NAVXChangeID").AsInt64().NotNullable()
                    .WithColumn("ResultCode").AsString()
                    .WithColumn("ErrorCode").AsString()
                    .WithColumn("Message").AsString()
                    .WithColumn("Tag").AsString()
                    .WithColumn("Value").AsString()
                    .WithColumn("Line").AsString();

            Create.Index("INX_NAVXResultXChangeID")
                          .OnTable("NAVXResult")
                          .OnColumn("NAVXChangeID").Ascending()
                          .WithOptions().NonClustered();

        }
    }
}
