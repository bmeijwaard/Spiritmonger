using Microsoft.EntityFrameworkCore;
using Spiritmonger.Core.Contracts.Messages;
using Spiritmonger.Persistence.Contracts;
using System;
using System.Threading.Tasks;

namespace Spiritmonger.Persistence.Base
{
    public abstract class BaseContext<TContext> where TContext : IBaseContext
    {
        protected readonly string _connectionString;

        public BaseContext(Func<TContext> context, string connectionString)
        {
            Context = context();
            _connectionString = connectionString;
        }

        /// <summary>
        /// Creates an isolated transactionscope with a new SqlContext and disposes the context afterwards. This ensures thread safety.
        /// </summary>
        /// <typeparam name="TResponse">The expected (service) response type</typeparam>
        /// <param name="contextFunc">The to be executed context mutations.</param>
        /// <returns>Returns the appropriate ServiceResponse.</returns>
        public async Task<IServiceResponse> ExecuteTransactionAsync<TResponse>(Func<TContext, Task<TResponse>> contextFunc) where TResponse : IServiceResponse
        {
            try
            {
                TResponse result = default;
                using (var contextTransaction = await _context.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        result = await contextFunc(_context).ConfigureAwait(false);
                        contextTransaction.Commit();

                        return result;
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return result;
                    }
                    catch (Exception e)
                    {
                        // TODO this is currently ignored.
                        if (e.Message.Contains("Database operation expected to affect 1 row(s) but actually affected 0 row(s)."))
                        {
                            return result;
                        }
                        contextTransaction.Rollback();
                        throw e;
                    }
                }
            }
            finally
            {
                _context.Dispose();
            }
        }


        public TContext Context { get; }

        private TContext _context
        {
            get
            {
                return InitializeContext();
            }
        }
        protected abstract TContext InitializeContext();
    }
}
