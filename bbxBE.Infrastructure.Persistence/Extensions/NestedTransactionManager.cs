using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Infrastructure.Persistence.Extensions
{
    public class NestedTransactionManager : IDbContextTransactionManager
    {
        readonly ISqlServerConnection _sqlServerConnection;

        public NestedTransactionManager(ISqlServerConnection sqlServerConnection)
        {
            // Dependency inject ISqlServerConnection, ISqlServerConnection is original IDbContextTransactionManager in EF Core 3.1 .
            _sqlServerConnection = sqlServerConnection;
        }

        public int Layer = 0;

        public IDbContextTransaction CurrentTransaction
            => _sqlServerConnection.CurrentTransaction;

        public IDbContextTransaction BeginTransaction()
        {
            if (Layer++ == 0)
                _sqlServerConnection.BeginTransaction();
            return new NestedTransation(this, Layer);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (Layer++ == 0)
                await _sqlServerConnection.BeginTransactionAsync(cancellationToken);
            return new NestedTransation(this, Layer);
        }

        public void CommitTransaction()
        {
            if (Layer-- <= 1)
                _sqlServerConnection.CommitTransaction();
        }

        public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (Layer-- <= 1)
            {
                return _sqlServerConnection.CurrentTransaction.CommitAsync(cancellationToken);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        public void ResetState()
            => _sqlServerConnection.ResetState();

        public Task ResetStateAsync(CancellationToken cancellationToken = default)
        {
            if (Layer-- <= 1)
            {
                return _sqlServerConnection.ResetStateAsync(cancellationToken);
            }
            else
            {
                return Task.CompletedTask;
            }
        }


        public void RollbackTransaction()
        {
            if (Layer-- <= 1)
            {
                _sqlServerConnection.RollbackTransaction();
            }
        }

        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (Layer-- <= 1)
            {
                return _sqlServerConnection.RollbackTransactionAsync(cancellationToken);
            }
            else
            {
                return Task.CompletedTask;
            }
        }
    }
}
