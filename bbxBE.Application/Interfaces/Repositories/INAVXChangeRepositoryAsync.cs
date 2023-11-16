using bbxBE.Common.Enums;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface INAVXChangeRepositoryAsync : IGenericRepositoryAsync<NAVXChange>
    {
        Task<IList<NAVXChange>> GetXChangeRecordsByStatus(enNAVStatus NAVStatus, int itemCnt);
        Task<IList<NAVXChange>> GetXChangeRecordByInvoiceID(long invoiceID);
        Task<NAVXChange> CreateNAVXChangeForManageInvoiceAsynch(Invoice invoice, CancellationToken cancellationToken);
    }
}