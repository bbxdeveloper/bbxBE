using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qInvoice;
using bbxBE.Common.Enums;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface INAVXChangeRepositoryAsync : IGenericRepositoryAsync<NAVXChange>
    {

        Task<NAVXChange> AddNAVXChangeAsync(NAVXChange NAVXChange);
        Task<NAVXChange> UpdateNAVXChangeAsync(NAVXChange NAVXChange);
        Task<NAVXChange> DeleteNAVXChangeAsync(long ID);
        Task<IList<NAVXChange>> GetXChangeRecordsByStatus(enNAVStatus NAVStatus, int itemCnt);
        Task<IList<NAVXChange>> GetXChangeRecordByInvoiceID(long invoiceID);
        Task<NAVXChange> CreateNAVXChangeForManageInvoiceAsynch(Invoice invoice, CancellationToken cancellationToken);
        Task<NAVXChange> CreateNAVXChangeForManageAnnulmentAsynch(Invoice invoice, CancellationToken cancellationToken);

        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedNAVXChangeAsync(QueryXChange requestParameter);

    }
}