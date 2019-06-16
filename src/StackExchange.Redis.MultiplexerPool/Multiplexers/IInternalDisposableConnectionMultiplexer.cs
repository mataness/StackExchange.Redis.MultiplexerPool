using System;
using System.Threading.Tasks;

namespace StackExchange.Redis.MultiplexerPool.Multiplexers
{
    /// <summary>
    /// An extended contract of <see cref="IConnectionMultiplexer"/> which adds methods that should be used internally for safely close the connection to
    /// the Redis server.
    /// A connection should be closed only when it's not a part of a connection pool.
    /// The traditional dispose methods defined in <see cref="IConnectionMultiplexer"/> will be blocked by the implementation, resulting in thrown
    /// <see cref="InvalidOperationException"/>.
    /// </summary>
    internal interface IInternalDisposableConnectionMultiplexer : IConnectionMultiplexer
    {
        /// <summary>
        /// Closes the connection to the Redis server
        /// </summary>
        /// <param name="allowCommandsToComplete">Whether to allow in-queue commands to complete first.</param>
        /// <returns>A <see cref="Task"/></returns>
        Task SafeCloseAsync(bool allowCommandsToComplete = true);

        /// <summary>
        /// Closes the connection to the Redis server
        /// </summary>
        /// <param name="allowCommandsToComplete">Whether to allow in-queue commands to complete first.</param>
        void SafeClose(bool allowCommandsToComplete = true);
    }
}
