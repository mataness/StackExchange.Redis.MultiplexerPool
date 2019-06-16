using System;
using System.Threading.Tasks;

namespace StackExchange.Redis.MultiplexerPool
{
    /// <summary>
    /// Represents a Redis <see cref="IConnectionMultiplexer"/> pool.
    /// All the connections within the pool share the same Redis server configuration.
    /// Each implementation has it's own strategy to return the next connection from the pool (e.g. using round robin strategy).
    /// The implementations are expected to implement <see cref="GetAsync"/> in a thread safe manner and optimized for performance.
    /// The implementation manages the connections to the Redis server and their lifetime.
    /// <remarks>
    /// Some implementations may lazily establish a connection to the Redis server (which means that at some point of time, the number of connections
    /// established to the Redis server may be lower than <see cref="PoolSize"/>.
    /// For that reason, <see cref="GetAsync"/> is asynchronous.
    /// </remarks>
    /// </summary>
    public interface IConnectionMultiplexerPool : IDisposable
    {
        /// <summary>
        /// Gets the size of the pool
        /// </summary>
        int PoolSize { get; }

        /// <summary>
        /// Gets a <see cref="IReconnectableConnectionMultiplexer"/> from the pool.
        /// </summary>
        /// <returns></returns>
        Task<IReconnectableConnectionMultiplexer> GetAsync();

        /// <summary>
        /// Closes all established connections in the pool.
        /// </summary>
        /// <param name="allowCommandsToComplete">Whether to allow in-queue commands to complete first.</param>
        /// <returns>A <see cref="Task"/></returns>
        Task CloseAllAsync(bool allowCommandsToComplete = true);
    }
}
