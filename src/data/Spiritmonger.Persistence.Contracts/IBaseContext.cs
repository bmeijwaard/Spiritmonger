using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Spiritmonger.Persistence.Contracts
{
    public interface IBaseContext
    {
        void Dispose();
        DbConnection GetConnection();
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        Task<IDbContextTransaction> BeginTransactionAsync();
        IDbContextTransaction BeginTransaction();
        IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel);
        EntityEntry Entry(object entity);
        EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class;
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        Task<int> ExecuteSqlCommandAsync(RawSqlString stp, params object[] parameters);

        Task BulkInsertOrUpdateAsync<T>(IList<T> entities) where T : class;

        Task BulkDeleteAsync<T>(IList<T> entities) where T : class;
    }
}
