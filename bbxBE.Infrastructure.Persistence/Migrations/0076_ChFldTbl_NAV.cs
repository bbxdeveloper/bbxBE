using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00076, "v00.02.37-NAVXChange Notice is nullable")]
    public class InitialTables_00076 : Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {
            Alter.Table("NAVXChange")
                    .AlterColumn("Notice").AsString(int.MaxValue).Nullable()
                    .AlterColumn("TokenTime").AsDateTime2().Nullable()
                    .AlterColumn("TokenRequest").AsString(int.MaxValue).Nullable()
                    .AlterColumn("Token").AsString().Nullable()
                    .AlterColumn("TokenResponse").AsString(int.MaxValue).Nullable()
                    .AlterColumn("TokenFuncCode").AsString().Nullable()                                     //OK,WARN,NULLTOKEN,EMPTYTOKEN
                    .AlterColumn("TokenMessage").AsString(int.MaxValue).Nullable()


                    .AlterColumn("SendTime").AsDateTime2().Nullable()
                    .AlterColumn("SendRequest").AsString(int.MaxValue).Nullable()
                    .AlterColumn("SendResponse").AsString(int.MaxValue).Nullable()
                    .AlterColumn("SendFuncCode").AsString().Nullable()                                     //OK,ERROR,POSTERROR
                    .AlterColumn("SendMessage").AsString(int.MaxValue).Nullable()

                    .AlterColumn("QueryTime").AsDateTime2().Nullable()
                    .AlterColumn("QueryRequest").AsString(int.MaxValue).Nullable()
                    .AlterColumn("QueryResponse").AsString(int.MaxValue).Nullable()
                    .AlterColumn("QueryFuncCode").AsString().Nullable()                                     //OK,ERROR,POSTERROR
                    .AlterColumn("QueryMessage").AsString(int.MaxValue).Nullable()
                    .AlterColumn("TransactionID").AsString().Nullable();

            Alter.Table("NAVXResult")
                    .AlterColumn("ResultCode").AsString().Nullable()
                    .AlterColumn("ErrorCode").AsString().Nullable()
                    .AlterColumn("Message").AsString().Nullable()
                    .AlterColumn("Tag").AsString().Nullable()
                    .AlterColumn("Value").AsString().Nullable()
                    .AlterColumn("Line").AsString().Nullable();
            Create.Index("INX_NAVXChangeStatus")
                        .OnTable("NAVXChange")
                        .OnColumn("Status").Ascending()
                        .OnColumn("ID").Ascending()
                        .WithOptions().NonClustered();

            Create.Index("INX_NAVXChangeInvoiceID")
                        .OnTable("NAVXChange")
                        .OnColumn("InvoiceID").Ascending()
                        .WithOptions().NonClustered();
        }
    }
}
