//using bbxBE.Application.Features.Positions.Queries.GetPositions;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qStockCard;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IStockCardRepositoryAsync : IGenericRepositoryAsync<StockCard>
    {
        Task<List<StockCard>> MaintainStockCardByInvoiceAsync(Invoice invoice);
        Task<Entity> GetStockCardAsync(GetStockCard requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedStockCardAsync(QueryStockCard requestParameters);
    }
}