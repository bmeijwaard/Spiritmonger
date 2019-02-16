using Spiritmonger.Cmon.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spiritmonger.Persistence.Contracts
{
    public interface IDapperProvider
    {
        Task<IEnumerable<T>> QueryCollectionAsync<T>(string storedProcedure, object parameters = null);
        Task<T> QuerySingleAsync<T>(string storedProcedure, object parameters = null);
        Task<int> ExecuteProcedureAsync(string storedProcedure, object parameters = null);
        Task<T0> QueryMultipleAsync<T0, T1>(string stp, object parameters);
        Task<T0> QueryMultipleAsync<T0, T1, T2>(string stp, object parameters);
        Task<T0> QueryMultipleAsync<T0, T1, T2, T3>(string stp, object parameters);
        Task<T0> QueryMultipleAsync<T0, T1, T2, T3, T4>(string stp, object parameters);
        Task<T0> QueryMultipleAsync<T0, T1, T2, T3>(string stp, object parameters, params Multiplicity[] transformation);
    }
}
