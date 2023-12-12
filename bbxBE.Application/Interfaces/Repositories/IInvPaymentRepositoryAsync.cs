using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qLocation;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static bxBE.Application.Commands.cmdLocation.CreateInvPaymentCommand;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IInvPaymentRepositoryAsync : IGenericRepositoryAsync<InvPayment>
    {
        Task<List<InvPayment>> MaintainRangeAsync(List<InvPaymentItem> InvPaymentItems);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedInvPaymentAsync(QueryInvPayment requestParameter, CancellationToken cancellationToken);

        Task<bool> SeedDataAsync(int rowCount);

    }
}