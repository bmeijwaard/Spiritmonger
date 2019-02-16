using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Spiritmonger.Cmon.Settings;
using Spiritmonger.Persistence.Base;
using Spiritmonger.Persistence.Contracts;
using System;

namespace Spiritmonger.Persistence.Providers
{
    public class SqlContextProvider : BaseContext<ISqlContext>, ISqlContextProvider
    {
        public SqlContextProvider(Func<SqlContext> context, IOptions<ConnectionStrings> connectionStrings)
            : base(context, connectionStrings.Value.DefaultConnection)
        {
        }

        protected override ISqlContext InitializeContext()
        {
            var builder = new DbContextOptionsBuilder<SqlContext>();
            builder.UseSqlServer(_connectionString);
            return new SqlContext(builder.Options);
        }
    }
}
