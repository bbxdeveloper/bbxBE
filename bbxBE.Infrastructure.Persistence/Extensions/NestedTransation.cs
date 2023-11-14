using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Infrastructure.Persistence.Extensions
{
    public class NestedTransation : IDbContextTransaction
    {
        readonly NestedTransactionManager _manager;

        readonly int _layer;

        public NestedTransation(NestedTransactionManager manager, int layer)
        {
            _manager = manager;
            _layer = layer;
        }

        bool Commited => _layer > _manager.Layer;

        public Guid TransactionId
            => Transaction.TransactionId;

        IDbContextTransaction Transaction
            => _manager.CurrentTransaction;

        public void Commit()
            => _manager.CommitTransaction();

        public Task CommitAsync(CancellationToken cancellationToken = default)
            => _manager.CommitTransactionAsync(cancellationToken);

        public void Dispose()
        {
            if (!Commited && Transaction != null)
                Transaction.Dispose();
        }

        public ValueTask DisposeAsync()
            => !Commited && Transaction != null
            ? Transaction.DisposeAsync()
            : default;

        public void Rollback()
            => _manager.RollbackTransaction();

        public Task RollbackAsync(CancellationToken cancellationToken = default)
            => _manager.RollbackTransactionAsync(cancellationToken);
    }
}
