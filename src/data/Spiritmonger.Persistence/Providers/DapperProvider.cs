using Spiritmonger.Cmon.Types;
using Spiritmonger.Persistence.Base;
using Spiritmonger.Persistence.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Spiritmonger.Persistence.Providers
{
    public class DapperProvider : DapperContext, IDapperProvider
    {
        public DapperProvider(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<T>> QueryCollectionAsync<T>(string storedProcedure, object parameters = null)
        {
            return await DapperConnectionAsync(async connection =>
            {
                var result = await connection.QueryAsync<T>(storedProcedure, parameters, null, 12, CommandType.StoredProcedure).ConfigureAwait(false);
                return result;
            });
        }

        public async Task<T> QuerySingleAsync<T>(string storedProcedure, object parameters = null)
        {
            return await DapperConnectionAsync(async connection =>
            {
                var result = await connection.QueryAsync<T>(storedProcedure, parameters, null, 12, CommandType.StoredProcedure).ConfigureAwait(false);
                return result.FirstOrDefault();
            });
        }

        public async Task<int> ExecuteProcedureAsync(string storedProcedure, object parameters = null)
        {
            return await DapperConnectionAsync(async connection => await connection.ExecuteAsync(storedProcedure, parameters, null, 12, CommandType.StoredProcedure)).ConfigureAwait(false);
        }

        private async Task<object[]> QueryMultipleAsync(string stp, object parameters, params Func<GridReader, Task<object>>[] readerFuncs)
        {
            var result = new dynamic[readerFuncs.Length];
            return await DapperConnectionAsync(async connection =>
            {
                try
                {
                    var gridReader = await connection.QueryMultipleAsync(stp, parameters, null, 120, CommandType.StoredProcedure).ConfigureAwait(false);

                    for (var index = 0; index < readerFuncs.Length; index++)
                    {
                        var obj = await readerFuncs[index](gridReader).ConfigureAwait(false);
                        result[index] = obj;
                    }

                    return result;
                }
                catch (InvalidOperationException e)
                {
                    // we catch this exception to supress a null/0 entry in the first select return.
                    if (e.Message.Contains("Sequence contains no elements"))
                    {
                        return result;
                    }
                    throw e;
                }
                catch (Exception e)
                {
                    throw e;
                }
            });
        }

        #region overloads
        public async Task<T0> QueryMultipleAsync<T0, T1>(string stp, object parameters)
        {
            var readers = new Func<GridReader, Task<object>>[2];

            await Task.Run(() =>
            {
                readers[0] = GetGridReader<T0>(Multiplicity.Single);
                readers[1] = GetGridReader<T1>(Multiplicity.Collection);
            });

            var response = await QueryMultipleAsync(stp, parameters, readers).ConfigureAwait(false);
            var result = (T0)response[0];
            if (result != null)
            {
                await Task.Run(() => { SetPropertyValue<T1, T0>(result, response[1]); }).ConfigureAwait(false);
            }

            return result;
        }

        public async Task<T0> QueryMultipleAsync<T0, T1, T2>(string stp, object parameters)
        {
            var readers = new Func<GridReader, Task<object>>[3];

            var task = Task.Run(() =>
            {
                readers[0] = GetGridReader<T0>(Multiplicity.Single);
                readers[1] = GetGridReader<T1>(Multiplicity.Collection);
                readers[2] = GetGridReader<T2>(Multiplicity.Collection);
            });
            await Task.WhenAll(task).ConfigureAwait(false);

            var response = await QueryMultipleAsync(stp, parameters, readers);
            var result = (T0)response[0];
            if (result != null)
            {
                task = Task.Run(() =>
                {
                    SetPropertyValue<T1, T0>(result, response[1]);
                    SetPropertyValue<T2, T0>(result, response[2]);
                });
                await Task.WhenAll(task).ConfigureAwait(false);
            }


            return result;
        }

        public async Task<T0> QueryMultipleAsync<T0, T1, T2, T3>(string stp, object parameters)
        {
            var readers = new Func<GridReader, Task<object>>[3];

            var task = Task.Run(() =>
            {
                readers[0] = GetGridReader<T0>(Multiplicity.Single);
                readers[1] = GetGridReader<T1>(Multiplicity.Collection);
                readers[2] = GetGridReader<T2>(Multiplicity.Collection);
                readers[3] = GetGridReader<T3>(Multiplicity.Collection);
            });
            await Task.WhenAll(task).ConfigureAwait(false);

            var response = await QueryMultipleAsync(stp, parameters, readers).ConfigureAwait(false);
            var result = (T0)response[0];
            if (result != null)
            {
                task = Task.Run(() =>
                {
                    SetPropertyValue<T1, T0>(result, response[1]);
                    SetPropertyValue<T2, T0>(result, response[2]);
                    SetPropertyValue<T3, T0>(result, response[3]);
                });
                await Task.WhenAll(task).ConfigureAwait(false);
            }


            return result;
        }

        public async Task<T0> QueryMultipleAsync<T0, T1, T2, T3, T4>(string stp, object parameters)
        {
            var readers = new Func<GridReader, Task<object>>[3];

            var task = Task.Run(() =>
            {
                readers[0] = GetGridReader<T0>(Multiplicity.Single);
                readers[1] = GetGridReader<T1>(Multiplicity.Collection);
                readers[2] = GetGridReader<T2>(Multiplicity.Collection);
                readers[3] = GetGridReader<T3>(Multiplicity.Collection);
                readers[4] = GetGridReader<T4>(Multiplicity.Collection);
            });
            await Task.WhenAll(task).ConfigureAwait(false);

            var response = await QueryMultipleAsync(stp, parameters, readers).ConfigureAwait(false);
            var result = (T0)response[0];
            if (result != null)
            {
                task = Task.Run(() =>
                {
                    SetPropertyValue<T1, T0>(result, response[1]);
                    SetPropertyValue<T2, T0>(result, response[2]);
                    SetPropertyValue<T3, T0>(result, response[3]);
                    SetPropertyValue<T4, T0>(result, response[4]);
                });
                await Task.WhenAll(task).ConfigureAwait(false);
            }

            return result;
        }

        public async Task<T0> QueryMultipleAsync<T0, T1, T2, T3>(string stp, object parameters, params Multiplicity[] transformation)
        {
            var readers = new Func<GridReader, Task<object>>[3];

            var task = Task.Run(() =>
            {
                readers[0] = GetGridReader<T0>(Multiplicity.Single);
                readers[1] = GetGridReader<T1>(transformation[0]);
                readers[2] = GetGridReader<T2>(transformation[1]);
                readers[3] = GetGridReader<T3>(transformation[2]);
            });
            await Task.WhenAll(task).ConfigureAwait(false);

            var response = await QueryMultipleAsync(stp, parameters, readers);
            var result = (T0)response[0];
            if (result != null)
            {
                task = Task.Run(() =>
                {
                    SetPropertyValue<T1, T0>(result, response[1]);
                    SetPropertyValue<T2, T0>(result, response[2]);
                });
                await Task.WhenAll(task).ConfigureAwait(false);
            }


            return result;
        }
        #endregion

        // private helper functions
        #region helpers
        private Func<GridReader, Task<object>> GetGridReader<T>(Multiplicity multiplicity)
        {
            switch (multiplicity)
            {
                case Multiplicity.Single:
                    return async reader => await reader.ReadFirstOrDefaultAsync<T>().ConfigureAwait(false);

                default:
                    return async reader => await reader.ReadAsync<T>().ConfigureAwait(false);
            }
        }

        private string GetTypeName<T>(Multiplicity multiplicity = Multiplicity.Collection)
        {
            var result = typeof(T).Name;
            switch (multiplicity)
            {
                case Multiplicity.Single:
                    return result;

                default:
                    return $"{result}s";
            }
        }

        private void SetPropertyValue<TChild, TParent>(TParent result, object input, string propName = null)
        {
            if (string.IsNullOrEmpty(propName))
            {
                propName = Clean(GetTypeName<TChild>());
            }
            typeof(TParent).GetProperty(propName).SetValue(result, ((IEnumerable)input).Cast<TChild>().ToList());
        }

        public string Clean(string input)
        {
            var sb = new StringBuilder(input);

            sb.Replace("DTO", string.Empty);
            sb.Replace("Base", string.Empty);
            sb.Replace("Aggregated", string.Empty);
            sb.Replace("Api", string.Empty);

            return sb.ToString();
        }
        #endregion
    }
}
