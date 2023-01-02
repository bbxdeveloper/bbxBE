
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qStockCard;
using bbxBE.Common.Enums;
using bbxBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IStockCardRepositoryAsync : IGenericRepositoryAsync<StockCard>
    {
        Task<StockCard> CreateStockCard(Stock p_Stock, DateTime p_StockCardDate,
            long p_WarehouseID, long? p_ProductID, long? p_UserID, long? p_InvoiceLineID, long? p_CustomerID,
            enStockCardType p_ScType, decimal p_XRealQty, decimal p_UnitPrice, string p_XRel);
        Task<Entity> GetStockCardAsync(long ID);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedStockCardAsync(QueryStockCard requestParameters);
    }
}