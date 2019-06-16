using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using StackExchange.Redis.MultiplexerPool;

namespace RedisConnectionPoolConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            RunExampleAsync().Wait();
        }

        public static async Task RunExampleAsync()
        {
            const string cRedisConnectionConfiguration = "REDIS_CONNECTION_CONFIG";
            var poolSize = 10;

            _connectionPool = ConnectionMultiplexerPoolFactory.Create(
                poolSize: poolSize,
                configuration: cRedisConnectionConfiguration,
                connectionSelectionStrategy: ConnectionSelectionStrategy.RoundRobin);

            _connectionsErrorCount = new int[poolSize];

            for (var i = 0; i < 100; i++)
            {
                var key = $"KEY_{i}";
                var value = $"KEY_{i}";
                await QueryRedisAsync(async db => await db.StringSetAsync(key, value));
            }

            for (var i = 0; i < 100; i++)
            {
                var key = $"KEY_{i}";
                var value = await QueryRedisAsync(async db => await db.StringGetAsync(key));

                Console.WriteLine($"Key: '{key}' Value: '{value}'");
            }
        }

        private static async Task<TResult> QueryRedisAsync<TResult>(Func<IDatabase, Task<TResult>> op)
        {
            var connection = await _connectionPool.GetAsync();

            Console.WriteLine($"Connection '{connection.ConnectionIndex}' established at {connection.ConnectionTimeUtc}");

            try
            {
                return await op(connection.Connection.GetDatabase());
            }
            catch (RedisConnectionException)
            {
                _connectionsErrorCount[connection.ConnectionIndex]++;

                if (_connectionsErrorCount[connection.ConnectionIndex] < 3)
                {
                    throw;
                }

                // Decide when to reconnect based on your own custom logic
                Console.WriteLine($"Re-establishing connection on index '{connection.ConnectionIndex}'");

                await connection.ReconnectAsync();

                return await op(connection.Connection.GetDatabase());
            }
        }

        private static IConnectionMultiplexerPool _connectionPool;
        private static int[] _connectionsErrorCount;
    }
}
