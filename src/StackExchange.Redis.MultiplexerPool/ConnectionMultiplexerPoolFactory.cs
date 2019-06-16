using System.ComponentModel;
using System.IO;
using StackExchange.Redis.MultiplexerPool.ConnectionSelection;
using StackExchange.Redis.MultiplexerPool.Infra.Common;
using StackExchange.Redis.MultiplexerPool.MultiplexerPools;
using StackExchange.Redis.MultiplexerPool.Multiplexers;

namespace StackExchange.Redis.MultiplexerPool
{
    /// <summary>
    /// A factory for <see cref="IConnectionMultiplexerPool"/>.
    /// This factory acts as the entry point for clients of this library and it should be used for creating a connection pool of <see cref="IConnectionMultiplexer"/>
    /// </summary>
    public static class ConnectionMultiplexerPoolFactory
    {
        /// <summary>
        /// Creates a new <see cref="IConnectionMultiplexerPool"/>.
        /// Connections to the Redis server are lazily established, this method doesn't perform any I/O operation such as network call.
        /// For more info refer to <see cref="IConnectionMultiplexerPool"/> interface docs
        /// </summary>
        /// <param name="poolSize">The size of the connection pool</param>
        /// <param name="configuration">The Redis connection string to use for establishing connections to the Redis server</param>
        /// <param name="textWriter">A <see cref="TextWriter"/> that will be use to write logs created by the <see cref="ConnectionMultiplexer"/></param>
        /// <param name="connectionSelectionStrategy">The connection selection strategy to be used by the connection pool</param>
        /// <returns>The created connection pool</returns>
        /// <remarks><see cref="IConnectionMultiplexerPool.GetAsync"/> is thread safe, for more info refer to the interface docs</remarks>
        public static IConnectionMultiplexerPool Create(
            int poolSize,
            string configuration,
            TextWriter textWriter = null,
            ConnectionSelectionStrategy connectionSelectionStrategy = ConnectionSelectionStrategy.LeastLoaded)
        {
            Guard.CheckArgumentLowerBound(poolSize, 0, nameof(poolSize));
            Guard.CheckNullArgument(configuration, nameof(configuration));

            var connectionFactory = new ConnectionMultiplexerFactory(configuration, textWriter);

            return CreateInternal(poolSize, connectionFactory, connectionSelectionStrategy);
        }

        /// <summary>
        /// Creates a new <see cref="IConnectionMultiplexerPool"/>.
        /// Connections to the Redis server are lazily established, this method doesn't perform any I/O operation such as network call.
        /// For more info refer to <see cref="IConnectionMultiplexerPool"/> interface docs
        /// </summary>
        /// <param name="poolSize">The size of the connection pool</param>
        /// <param name="configurationOptions">The Redis connection configuration to use for establishing connections to the Redis server</param>
        /// <param name="textWriter">A <see cref="TextWriter"/> that will be use to write logs created by the <see cref="ConnectionMultiplexer"/></param>
        /// <param name="connectionSelectionStrategy">The connection selection strategy to be used by the connection pool</param>
        /// <returns>The created connection pool</returns>
        /// <remarks><see cref="IConnectionMultiplexerPool.GetAsync"/> is thread safe, for more info refer to the interface docs</remarks>
        public static IConnectionMultiplexerPool Create(
            int poolSize,
            ConfigurationOptions configurationOptions,
            TextWriter textWriter = null,
            ConnectionSelectionStrategy connectionSelectionStrategy = ConnectionSelectionStrategy.RoundRobin)
        {
            Guard.CheckArgumentLowerBound(poolSize, 0, nameof(poolSize));
            Guard.CheckNullArgument(configurationOptions, nameof(connectionSelectionStrategy));

            var connectionFactory = new ConnectionMultiplexerFactory(configurationOptions, textWriter);

            return CreateInternal(poolSize, connectionFactory, connectionSelectionStrategy);
        }

        private static IConnectionMultiplexerPool CreateInternal(
            int poolSize,
            IConnectionMultiplexerFactory connectionFactory,
            ConnectionSelectionStrategy connectionSelectionStrategy)
        {
            IConnectionSelector strategy;

            switch (connectionSelectionStrategy)
            {
                case ConnectionSelectionStrategy.RoundRobin:
                    strategy = new RoundRobinConnectionSelector();
                    break;

                case ConnectionSelectionStrategy.LeastLoaded:
                    strategy = new LoadBasedConnectionSelector();
                    break;

                default:
                    throw new InvalidEnumArgumentException(nameof(connectionSelectionStrategy), (int)connectionSelectionStrategy, typeof(ConnectionSelectionStrategy));
            }

            return new ConnectionMultiplexerPool(poolSize, new ReconnectableConnectionMultiplexerFactory(connectionFactory), strategy);
        }
    }
}
