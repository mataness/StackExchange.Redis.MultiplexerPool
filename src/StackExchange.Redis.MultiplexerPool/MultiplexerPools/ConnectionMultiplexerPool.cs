using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;
using StackExchange.Redis.MultiplexerPool.ConnectionSelection;
using StackExchange.Redis.MultiplexerPool.Infra.Common;
using StackExchange.Redis.MultiplexerPool.Multiplexers;

namespace StackExchange.Redis.MultiplexerPool.MultiplexerPools
{
    /// <summary>
    /// Implements <see cref="IConnectionMultiplexerPool"/>.
    /// The implementation selects the connection to return from <see cref="GetAsync"/> using the given <see cref="IConnectionSelector"/>
    /// The connections are lazily established.
    /// The amount of established connections will be equal to <see cref="PoolSize"/> after <see cref="PoolSize"/> calls to
    /// <see cref="GetAsync"/>.
    /// </summary>
    internal class ConnectionMultiplexerPool : IConnectionMultiplexerPool
    {
        internal ConnectionMultiplexerPool(
            int poolSize,
            IReconnectableConnectionMultiplexerFactory connectionFactory,
            IConnectionSelector connectionSelector)
        {
            PoolSize = Guard.CheckArgumentLowerBound(poolSize, 1, nameof(poolSize));
            Guard.CheckNullArgument(connectionFactory, nameof(connectionFactory));
            Guard.CheckNullArgument(connectionSelector, nameof(connectionSelector));

            _establishedConnections = new ConcurrentQueue<IInternalReconnectableConnectionMultiplexer>();
            _connectionFactory = connectionFactory;
            _connectionCreationLock = new AsyncLock();
            _connectionSelector = connectionSelector;
        }
        /// <inheritdoc />
        public int PoolSize { get; }

        /// <inheritdoc />
        public async Task<IReconnectableConnectionMultiplexer> GetAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ConnectionMultiplexerPool));
            }

            if (_establishedConnectionsAsArr == null)
            {
                using (await _connectionCreationLock.LockAsync().ConfigureAwait(false))
                {
                    if (_establishedConnectionsAsArr == null)
                    {
                        var connection = await _connectionFactory.CreateAsync(_establishedConnections.Count).ConfigureAwait(false);

                        _establishedConnections.Enqueue(connection);

                        if (_establishedConnections.Count == PoolSize)
                        {
                            _establishedConnectionsAsArr = _establishedConnections.ToArray();
                        }

                        return connection;
                    }
                }
            }

            return _connectionSelector.Select(_establishedConnectionsAsArr);
        }

        /// <inheritdoc />
        public Task CloseAllAsync(bool allowCommandsToComplete = true)
        {
            if (_disposed)
            {
                return Task.CompletedTask;
            }

            _disposed = true;

            return Task.WhenAll(_establishedConnections.Select(connection => connection.Connection.SafeCloseAsync(allowCommandsToComplete)));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            foreach (var connectionToDispose in _establishedConnections)
            {
                connectionToDispose.Connection.SafeClose();
            }
        }

        private readonly IConnectionSelector _connectionSelector;
        private readonly ConcurrentQueue<IInternalReconnectableConnectionMultiplexer> _establishedConnections;
        private readonly AsyncLock _connectionCreationLock;
        private readonly IReconnectableConnectionMultiplexerFactory _connectionFactory;
        private IInternalReconnectableConnectionMultiplexer[] _establishedConnectionsAsArr;
        private bool _disposed;

    }
}
