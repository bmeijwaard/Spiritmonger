using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Common;
using EFCore.BulkExtensions;
using Spiritmonger.Domain.Entities;
using Spiritmonger.Persistence.Contracts;

namespace Spiritmonger.Persistence
{
    public class SqlContext : DbContext, ISqlContext
    {
        public SqlContext(DbContextOptions<SqlContext> options) : base(options)
        {
        }

        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<MetaCard> MetaCards { get; set; }
        public virtual DbSet<CardName> CardNames { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }


        /// <summary>
        /// Start a transactionscope used to control multiple mutations
        /// </summary>
        /// <returns></returns>
        public IDbContextTransaction BeginTransaction()
        {
            return Database.BeginTransaction();
        }

        /// <summary>
        /// Start a transactionscope used to control multiple mutations
        /// </summary>
        /// <returns></returns>
        public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return Database.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Start a transactionscope used to control multiple mutations
        /// </summary>
        /// <returns></returns>
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await Database.BeginTransactionAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve the current DbConnection
        /// </summary>
        /// <returns>DbConnection</returns>
        public DbConnection GetConnection()
        {
            return Database.GetDbConnection();
        }

        /// <summary>
        /// Override to hide the parameter for cancellation tokens iot prevent mannually setting an empty token for every savechanges.
        /// </summary>
        /// <returns>int  result.</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Excecutes (custom) raw stored-procedures, use return parameters to retrieve information back from the STP.
        /// </summary>
        /// <param name="stp">The STP.</param>
        /// <param name="parameters">Array of SqlParameter.</param>
        /// <returns>int result.</returns>
        public async Task<int> ExecuteSqlCommandAsync(RawSqlString stp, params object[] parameters)
        {
            return await Database.ExecuteSqlCommandAsync(stp, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Bulk insert or update of entities.
        /// </summary>
        /// <typeparam name="T">The Type of the collection of entities</typeparam>
        /// <param name="entities">The collection of entities</param>
        /// <returns></returns>
        public async Task BulkInsertOrUpdateAsync<T>(IList<T> entities) where T : class
        {
            await DbContextBulkExtensions.BulkInsertOrUpdateAsync(this, entities).ConfigureAwait(false);
        }

        /// <summary>
        /// Bulk delete entities.
        /// </summary>
        /// <typeparam name="T">The Type of the collection of entities</typeparam>
        /// <param name="entities">The collection of entities</param>
        /// <returns></returns>
        public async Task BulkDeleteAsync<T>(IList<T> entities) where T : class
        {
            await DbContextBulkExtensions.BulkDeleteAsync(this, entities).ConfigureAwait(false);
        }


    }
}

