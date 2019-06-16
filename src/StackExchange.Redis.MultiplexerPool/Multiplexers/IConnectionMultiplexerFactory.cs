using System.Threading.Tasks;

namespace StackExchange.Redis.MultiplexerPool.Multiplexers
{
    /// <summary>
    /// A factory for <see cref="IInternalDisposableConnectionMultiplexer"/>
    /// </summary>
    internal interface IConnectionMultiplexerFactory
    {
        /// <summary>
        /// Creates a new <see cref="IInternalDisposableConnectionMultiplexer"/>.
        /// The created instance is already connected to the Redis server.
        /// </summary>
        /// <returns>The created <see cref="IInternalDisposableConnectionMultiplexer"/></returns>
        Task<IInternalDisposableConnectionMultiplexer> CreateAsync();
    }
}
