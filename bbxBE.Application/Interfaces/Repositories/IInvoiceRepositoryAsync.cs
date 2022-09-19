using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qInvoice;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IInvoiceRepositoryAsync : IGenericRepositoryAsync<Invoice>
    {
        Task<bool> IsUniqueInvoiceNumberAsync(string InvoiceNumber, long? ID = null);
        Task<bool> SeedDataAsync(int rowCount);
        Task<Invoice> AddInvoiceAsync(Invoice p_invoice);
        Task<Invoice> UpdateInvoiceAsync(Invoice p_invoice);

        Task<Entity> GetInvoiceAsync( long ID , bool FullData);
        Task<IEnumerable<Entity>> GetPendigDeliveryNotesSummareAsync(bool Incoming, long WarehouseID, string CurrencyCode);
        Task<Invoice> GetInvoiceRecordAsync(long ID, bool FullData = true);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedInvoiceAsync(QueryInvoice requestParameters);
        
    }
}