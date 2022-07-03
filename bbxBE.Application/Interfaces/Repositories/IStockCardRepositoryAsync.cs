//using bbxBE.Application.Features.Positions.Queries.GetPositions;
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
        Task<StockCard> CreateStockCard(DateTime StockCardDate, long StockID,
            long WarehouseID, long? ProductID, long? UserID, long? InvoiceLineID, long? CustomerID,
            enStockCardType ScType,
            decimal OCalcQty, decimal ORealQty,
            decimal XCalcQty, decimal XRealQty,
            decimal OAvgCost, decimal NAvgCost,
            string XRel);
        Task<Entity> GetStockCardAsync(GetStockCard requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedStockCardAsync(QueryStockCard requestParameters);
    }
}