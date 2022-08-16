//using bbxBE.Application.Features.Positions.Queries.GetPositions;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qStock;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IStockRepositoryAsync : IGenericRepositoryAsync<Stock>
    {
        Task<List<Stock>> MaintainStockByInvoiceAsync(Invoice invoice);
        Task<Entity> GetStockAsync(GetStock requestParameters);
        Task<Stock> GetStockRecordAsync(GetStockRecord requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedStockAsync(QueryStock requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryInvCtrlStockAbsentAsync(QueryInvCtrlStockAbsent requestParameters);


    }
}