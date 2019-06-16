using System.Threading.Tasks;

namespace StackExchange.Redis.MultiplexerPool.Multiplexers
{
    /// <summary>
    /// Implements <see cref="IReconnectableConnectionMultiplexerFactory"/>.
    /// Creates instances of <see cref="ReconnectableConnectionMultiplexer"/>.
    /// </summary>
    internal class ReconnectableConnectionMultiplexerFactory : IReconnectableConnectionMultiplexerFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReconnectableConnectionMultiplexerFactory"/> class.
        /// </summary>
        /// <param name="connectionMultiplexerFactory">Will be used to create the <see cref="IInternalDisposableConnectionMultiplexer"/> wrapped by the <see cref="ReconnectableConnectionMultiplexer"/></param>
        public ReconnectableConnectionMultiplexerFactory(IConnectionMultiplexerFactory connectionMultiplexerFactory)
        {
            _connectionMultiplexerFactory = connectionMultiplexerFactory;
        }

        /// <inheritdoc />
        public async Task<IInternalReconnectableConnectionMultiplexer> CreateAsync(int connectionIndex)
        {
            var connection = await _connectionMultiplexerFactory.CreateAsync().ConfigureAwait(false);

            return new ReconnectableConnectionMultiplexer(connectionIndex, connection, _connectionMultiplexerFactory);
        }

        private readonly IConnectionMultiplexerFactory _connectionMultiplexerFactory;
    }
}
