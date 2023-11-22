using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00077, "v00.02.37-GreatReset")]
    public class InitialTables_00077 : Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {
            var sql = "CREATE PROCEDURE SetCounter(														\n" +
            "	@warehouseID int,                                                                                                               \n" +
            "	@currentNumber int                                                                                                              \n" +
            ")                                                                                                                                      \n" +
            "as begin                                                                                                                               \n" +
            "INSERT INTO [dbo].[Counter] ([WarehouseID], [CounterCode] ,[CounterDescription] ,[Prefix] ,[CurrentNumber],[NumbepartLength] ,[Suffix])\n" +
            "VALUES ( @warehouseID, 'AJ_00'+ cast( @warehouseID as char), 'Ajánlat', 'AJ', @currentNumber, 5, '/')                                                              \n" +
            "INSERT INTO [dbo].[Counter] ([WarehouseID], [CounterCode] ,[CounterDescription] ,[Prefix] ,[CurrentNumber],[NumbepartLength] ,[Suffix])\n" +
            "VALUES ( @warehouseID, 'BEV_00'+ cast( @warehouseID as char), 'Bevételezés számlán', 'BEV', @currentNumber, 5, '/')                                                \n" +
            "INSERT INTO [dbo].[Counter] ([WarehouseID], [CounterCode] ,[CounterDescription] ,[Prefix] ,[CurrentNumber],[NumbepartLength] ,[Suffix])\n" +
            "VALUES ( @warehouseID, 'BES_00'+ cast( @warehouseID as char), 'Bevételezés szállítólevélen', 'BES', @currentNumber, 5, '/')                                        \n" +
            "INSERT INTO [dbo].[Counter] ([WarehouseID], [CounterCode] ,[CounterDescription] ,[Prefix] ,[CurrentNumber],[NumbepartLength] ,[Suffix])\n" +
            "VALUES ( @warehouseID, 'BEJ_00'+ cast( @warehouseID as char), 'Bevételezés javítószámla', 'BEJ', @currentNumber, 5, '/')                                           \n" +
            "INSERT INTO [dbo].[Counter] ([WarehouseID], [CounterCode] ,[CounterDescription] ,[Prefix] ,[CurrentNumber],[NumbepartLength] ,[Suffix])\n" +
            "VALUES ( @warehouseID, 'BLK_00'+ cast( @warehouseID as char), 'Blokk KP', 'BLK', @currentNumber, 5, '/')                                                           \n" +
            "INSERT INTO [dbo].[Counter] ([WarehouseID], [CounterCode] ,[CounterDescription] ,[Prefix] ,[CurrentNumber],[NumbepartLength] ,[Suffix])\n" +
            "VALUES ( @warehouseID, 'BLC_00'+ cast( @warehouseID as char), 'Blokk kártya', 'BLC', @currentNumber, 5, '/')                                                       \n" +
            "INSERT INTO [dbo].[Counter] ([WarehouseID], [CounterCode] ,[CounterDescription] ,[Prefix] ,[CurrentNumber],[NumbepartLength] ,[Suffix])\n" +
            "VALUES ( @warehouseID, 'K_00'+ cast( @warehouseID as char), 'KP eladás számla', 'K-', (@currentNumber-1)*10+1, 6, '/')                                                    \n" +
            "INSERT INTO [dbo].[Counter] ([WarehouseID], [CounterCode] ,[CounterDescription] ,[Prefix] ,[CurrentNumber],[NumbepartLength] ,[Suffix])\n" +
            "VALUES ( @warehouseID, 'A_00'+ cast( @warehouseID as char), 'Átutalás eladás számla', 'A-', (@currentNumber-1)*10+1, 6, '/')                                              \n" +
            "INSERT INTO [dbo].[Counter] ([WarehouseID], [CounterCode] ,[CounterDescription] ,[Prefix] ,[CurrentNumber],[NumbepartLength] ,[Suffix])\n" +
            "VALUES ( @warehouseID, 'C_00'+ cast( @warehouseID as char), 'Bankkártya eladás számla', 'C-', (@currentNumber-1)*10+1, 6, '/')                                            \n" +
            "INSERT INTO [dbo].[Counter] ([WarehouseID], [CounterCode] ,[CounterDescription] ,[Prefix] ,[CurrentNumber],[NumbepartLength] ,[Suffix])\n" +
            "VALUES ( @warehouseID, 'S_00'+ cast( @warehouseID as char), 'Kimenő szállítólevél', 'S-', (@currentNumber-1)*10+1, 6, '/')                                                \n" +
            "INSERT INTO [dbo].[Counter] ([WarehouseID], [CounterCode] ,[CounterDescription] ,[Prefix] ,[CurrentNumber],[NumbepartLength] ,[Suffix])\n" +
            "VALUES ( @warehouseID, 'JAV_00'+ cast( @warehouseID as char), 'Kimenő javítószámla', 'JAV', @currentNumber, 5, '/')                                                \n" +
            "INSERT INTO [dbo].[Counter] ([WarehouseID], [CounterCode] ,[CounterDescription] ,[Prefix] ,[CurrentNumber],[NumbepartLength] ,[Suffix])\n" +
            "VALUES ( @warehouseID, 'WHT_00'+ cast( @warehouseID as char), 'Raktárközi bizonylat', 'WHT', @currentNumber, 5, '/')                                               \n" +
            "end                                                                                                                                    ";

            Execute.Sql(sql);

            sql = "truncate table AdditionalInvoiceData    \n" +
                    "truncate table AdditionalInvoiceLineData\n" +
                    "truncate table [counter]                \n" +
                    "truncate table InvCtrl                  \n" +
                    "truncate table InvCtrlPeriod            \n" +
                    "truncate table Invoice                  \n" +
                    "truncate table InvoiceLine              \n" +
                    "truncate table NAVXChange               \n" +
                    "truncate table NAVXResult               \n" +
                    "truncate table Offer                    \n" +
                    "truncate table OfferLine                \n" +
                    "truncate table Stock                    \n" +
                    "truncate table StockCard                \n" +
                    "truncate table SummaryByVatRate         \n" +
                    "truncate table Warehouse                \n" +
                    "truncate table WhsTransfer              \n" +
                    "truncate table WhsTransferLine          ";

            Execute.Sql(sql);

            sql = "insert into Warehouse ([WarehouseCode],[WarehouseDescription]) values ('001', 'Szolnok')		\n" +
                    "execute SetCounter 1, 1                                                                                \n" +
                    "insert into Warehouse ([WarehouseCode],[WarehouseDescription]) values ('002', 'Kecskemét')             \n" +
                    "execute SetCounter 2, 50001                                                                            \n" +
                    "insert into Warehouse ([WarehouseCode],[WarehouseDescription]) values ('003', 'Nagykőrös')             \n" +
                    "execute SetCounter 3, 80001                                                                            \n" +
                    "insert into Warehouse ([WarehouseCode],[WarehouseDescription]) values ('004', 'Cukorgyár')             \n" +
                    "execute SetCounter 4, 90001                                                                            \n" +
                    "insert into Warehouse ([WarehouseCode],[WarehouseDescription]) values ('005', 'Kecskemét külső raktár')\n" +
                    "execute SetCounter 5, 9500                                                                             ";

            Execute.Sql(sql);

            sql = "drop PROCEDURE SetCounter";
            Execute.Sql(sql);

        }
    }
}
