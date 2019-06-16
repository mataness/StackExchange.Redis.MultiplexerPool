using System;
using System.Threading.Tasks;

namespace StackExchange.Redis.MultiplexerPool
{
    /// <summary>
    /// Wraps a <see cref="IConnectionMultiplexer"/> and adds functionality to reconnect an existing connection to the Redis server.
    /// This functionality is useful in cases where the established connection is no longer stable.
    /// This has been reported by users such as here https://github.com/StackExchange/StackExchange.Redis/issues/1120 
    /// </summary>
    public interface IReconnectableConnectionMultiplexer
    {
        /// <summary>
        /// Gets the <see cref="IConnectionMultiplexer"/> that is already connected to the Redis server
        /// </summary>
        IConnectionMultiplexer Connection { get; }

        /// <summary>
        /// Gets the index of the connection (a value a value between 0 to PoolSize - 1)
        /// </summary>
        int ConnectionIndex { get; }

        /// <summary>
        /// Gets the time that the wrapped connection was established or reconnected
        /// </summary>
        DateTime ConnectionTimeUtc { get; }

        /// <summary>
        /// Creates a new wrapped <see cref="Connection"/>.
        /// First, a new connection is being established and only after that, the previous connection is closed.
        /// Calls to <see cref="Connection"/> from other threads will result in returning the previous connection until a new connection has been established.
        /// Concurrent calls to this method will be ignored and only the first call will be respected in order to avoid unwanted multiple reconnectios.
        /// </summary>
        /// <param name="allowCommandsToComplete">Whether to allow in-queue commands to complete first.</param>
        /// <param name="fireAndForgetOnClose">Whether to wait for the previous connection to close before returning to the caller</param>
        /// <returns>A <see cref="Task"/></returns>
        /// <remarks>
        /// <see cref="Connection"/> will never return a <see cref="IConnectionMultiplexer"/> that was already disposed
        /// (even when <see cref="Connection"/> is called from other threads when this method is being executed)
        /// </remarks>
        Task ReconnectAsync(bool allowCommandsToComplete = true, bool fireAndForgetOnClose = true);
    }
}
