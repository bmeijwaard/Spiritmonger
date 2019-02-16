using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Spiritmonger.Persistence.Base
{
    public abstract class DapperContext
    {
        private readonly string _connectionString;

        protected DapperContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected async Task<T> DapperConnectionAsync<T>(Func<IDbConnection, Task<T>> queryFunc)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                return await queryFunc(connection).ConfigureAwait(false);
            }
        }
    }
}
