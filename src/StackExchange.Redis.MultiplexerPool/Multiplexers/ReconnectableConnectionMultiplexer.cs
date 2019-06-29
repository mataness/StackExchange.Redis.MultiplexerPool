using System;
using System.Threading.Tasks;
using Nito.AsyncEx;
using StackExchange.Redis.MultiplexerPool.Infra.Common;

#pragma warning disable 4014

namespace StackExchange.Redis.MultiplexerPool.Multiplexers
{
    /// <summary>
    /// Implements <see cref="IInternalReconnectableConnectionMultiplexer"/>.
    /// </summary>
    internal class ReconnectableConnectionMultiplexer : IInternalReconnectableConnectionMultiplexer
    {
        public ReconnectableConnectionMultiplexer(
            int connectionIndex,
            IInternalDisposableConnectionMultiplexer connectionMultiplexer,
            IConnectionMultiplexerFactory connectionMultiplexerFactory)
        {
            ConnectionIndex = Guard.CheckArgumentLowerBound(connectionIndex, 0, nameof(connectionIndex));
            ConnectionTimeUtc = DateTime.UtcNow;
            _connectLock = new AsyncLock();
            Connection = Guard.CheckNullArgument(connectionMultiplexer, nameof(connectionMultiplexer));
            _connectionMultiplexerFactory = Guard.CheckNullArgument(connectionMultiplexerFactory, nameof(connectionMultiplexerFactory));
        }

        /// <inheritdoc />
        IConnectionMultiplexer IReconnectableConnectionMultiplexer.Connection => Connection;

        /// <inheritdoc />
        public IInternalDisposableConnectionMultiplexer Connection { get; private set; }

        /// <inheritdoc />
        public int ConnectionIndex { get; }

        /// <inheritdoc />
        public DateTime ConnectionTimeUtc { get; private set; }

        /// <inheritdoc />
        public async Task ReconnectAsync(bool allowCommandsToComplete = true, bool fireAndForgetOnClose = true)
        {
            var connectionTimeUtc = ConnectionTimeUtc;
            using (await _connectLock.LockAsync().ConfigureAwait(false))
            {
                if (connectionTimeUtc == ConnectionTimeUtc)
                {
                    var previousConnection = Connection;

                    Connection = await _connectionMultiplexerFactory.CreateAsync();

                    if (fireAndForgetOnClose)
                    {
                        previousConnection.SafeCloseAsync(allowCommandsToComplete);
                    }
                    else
                    {
                        await previousConnection.SafeCloseAsync(allowCommandsToComplete).ConfigureAwait(false);
                    }

                    ConnectionTimeUtc = DateTime.UtcNow;
                }
            }
        }

        private readonly IConnectionMultiplexerFactory _connectionMultiplexerFactory;
        private readonly AsyncLock _connectLock;
    }
}
