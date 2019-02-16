using Spiritmonger.Core.Contracts.Messages;
using System;
using System.Threading.Tasks;

namespace Spiritmonger.Persistence.Contracts
{
    public interface ISqlContextProvider
    {
        ISqlContext Context { get; }
        Task<IServiceResponse> ExecuteTransactionAsync<T>(Func<ISqlContext, Task<T>> contextFunc) where T : IServiceResponse;
    }
}
