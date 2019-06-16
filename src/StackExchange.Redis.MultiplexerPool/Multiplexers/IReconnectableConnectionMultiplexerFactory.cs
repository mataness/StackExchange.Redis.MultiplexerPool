using System.Threading.Tasks;

namespace StackExchange.Redis.MultiplexerPool.Multiplexers
{
    /// <summary>
    /// A factory for <see cref="IInternalReconnectableConnectionMultiplexer"/>
    /// </summary>
    internal interface IReconnectableConnectionMultiplexerFactory
    {
        /// <summary>
        /// Creates a new <see cref="IInternalReconnectableConnectionMultiplexer"/>.
        /// The created instance is already connected to the Redis server.
        /// </summary>
        /// <returns>The created connection</returns>
        Task<IInternalReconnectableConnectionMultiplexer> CreateAsync(int connectionIndex);
    }
}
