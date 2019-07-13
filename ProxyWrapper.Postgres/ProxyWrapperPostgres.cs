using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Dapper;
using Newtonsoft.Json;

namespace ProxyWrapper
{
    public class ProxyWrapperPostgres: IProxyWrapperStorage
    {
        private readonly string _connectionString;
        private JsonSerializerSettings _jsonSettigs;

        public ProxyWrapperPostgres(string connectionString)
        {
            _connectionString = connectionString;

            TryInitDatabase();

            _jsonSettigs = new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.All};
        }

        private void TryInitDatabase()
        {
            using (var conn = CreateConnection())
            {
                string database = conn.Database;

                conn.Execute(@"
                            CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"";
                    
                            create table if not exists surrogates (
                                id uuid primary key, 
                                WrappedService text not null,
                                Method text not null,
                                Args text,
                                Response text, 
                                ActiveMock boolean
                            );
                            
                            create index if not exists idx_surrogates_service_method on surrogates(WrappedService, Method);
                            ");
            }
        }

        public bool Invoke(InvokeCommand invokeCommad, out object result)
        {
            string jsonArgs = JsonConvert.SerializeObject(invokeCommad.Args);

            using (var conn = CreateConnection())
            {
                var callRes = GetCallRes(invokeCommad, conn, jsonArgs);
                
                if (callRes != null && callRes.ActiveMock)
                {
                    result = JsonConvert.DeserializeObject(callRes.Response, _jsonSettigs);
                    return true;
                }
            }

            result = null;
            return false;
        }

        private static MethodCall GetCallRes(InvokeCommand invokeCommad, NpgsqlConnection conn, string jsonArgs)
        {
            return conn.QueryFirstOrDefault<MethodCall>(
                "select * from surrogates where WrappedService = @Service and Method = @Method AND Args = @jsonArgs",
                new
                {
                    Service = invokeCommad.WrappedType.FullName,
                    Method = invokeCommad.Binder.Name,
                    jsonArgs
                });
        }

        public void LastResult(InvokeCommand invokeCommad, object result)
        {
            string jsonArgs = JsonConvert.SerializeObject(invokeCommad.Args);

            using (var conn = CreateConnection())
            {
                var callRes = GetCallRes(invokeCommad, conn, jsonArgs);

                if (callRes != null)
                    return;

                conn.Execute(@"insert into surrogates (id, WrappedService, Method, Args, Response, ActiveMock)
                                                    values (uuid_generate_v4(), @Service, @Method, @jsonArgs, @result, false)",
                    new
                    {
                        Service = invokeCommad.WrappedType.FullName,
                        Method = invokeCommad.Binder.Name,
                        result = JsonConvert.SerializeObject(result, Formatting.Indented, _jsonSettigs),
                        jsonArgs
                    });
            }
        }

        public Task<IEnumerable<Interface>> GetInterfaces()
        {
            return Exec(conn => conn.QueryAsync<Interface>("SELECT WrappedService, sum(activemock::int) activemocks from surrogates group by 1 order by 1"));
        }

        private async Task<T> Exec<T>(Func<NpgsqlConnection, Task<T>> func)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                return await func(conn);
            }
        }

        private NpgsqlConnection CreateConnection()
        {
            var conn = new NpgsqlConnection(_connectionString);

            conn.Open();
            
            return conn;
        }
    }
    
    internal class MethodCall
    {
        public string WrappedService { get; set; }
        
        public string Method { get; set; }

        public string Args { get; set; }

        public string Response { get; set; }

        public bool ActiveMock { get; set; }
    }
}