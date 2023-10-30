using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qWhsTransfer;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdWhsTransfer;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IWhsTransferRepositoryAsync : IGenericRepositoryAsync<WhsTransfer>
    {
        Task<WhsTransfer> AddWhsTransferAsync(WhsTransfer whsTransfer);
        Task<WhsTransfer> UpdateWhsTransferAsync(WhsTransfer whsTransfer);
        Task<WhsTransfer> DeleteWhsTransferAsync(long ID);

        Task<Entity> GetWhsTransferAsync(long ID);
        Task<WhsTransfer> GetWhsTransferRecordAsync(long ID, bool fulldata);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedWhsTransferAsync(QueryWhsTransfer requestParameter);
        Task<WhsTransfer> ProcessAsync(long ID, DateTime transferDateIn);
        Task<WhsTransfer> CreateWhsTransferAsynch(CreateWhsTransferCommand request, CancellationToken cancellationToken);
        Task<WhsTransfer> UpdateWhsTransferAsynch(UpdateWhsTransferCommand request, CancellationToken cancellationToken);

    }
}