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
        Task<List<Stock>> MaintainStockByInvCtrlAsync(List<InvCtrl> invCtrlList, Customer p_ownData, string XRel);

        Task<Entity> GetStockAsync(long ID);
        Task<Stock> GetStockRecordAsync(long warehouseID, long productID);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedStockAsync(QueryStock requestParameters);
   
        Task<Stock> UpdateStockLocationAsync( long ID, long? LocationID);

    }
}