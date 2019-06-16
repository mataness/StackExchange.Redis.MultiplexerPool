using System.IO;
using System.Threading.Tasks;
using StackExchange.Redis.MultiplexerPool.Infra.Common;

namespace StackExchange.Redis.MultiplexerPool.Multiplexers
{
    /// <summary>
    /// Implements <see cref="IConnectionMultiplexerFactory"/>.
    /// Creates instances of <see cref="InternalDisposableConnectionMultiplexer"/>.
    /// The implementation establishes a connection to the Redis server using <see cref="ConnectionMultiplexer"/> ConnectAsync methods.
    /// </summary>
    internal class ConnectionMultiplexerFactory : IConnectionMultiplexerFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMultiplexerFactory"/> class.
        /// </summary>
        /// <param name="configuration">The Redis connection string to use for establishing connections to the Redis server</param>
        /// <param name="textWriter">A <see cref="TextWriter"/> that will be use to write logs created by the <see cref="ConnectionMultiplexer"/></param>
        public ConnectionMultiplexerFactory(string configuration, TextWriter textWriter = null)
        {
            _configuration = Guard.CheckNullArgument(configuration, nameof(configuration));
            _textWriter = textWriter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMultiplexerFactory"/> class.
        /// </summary>
        /// <param name="configurationOptions">The Redis connection configuration to use for establishing connections to the Redis server</param>
        /// <param name="textWriter">A <see cref="TextWriter"/> that will be use to write logs created by the <see cref="ConnectionMultiplexer"/></param>
        public ConnectionMultiplexerFactory(ConfigurationOptions configurationOptions, TextWriter textWriter = null)
        {
            _configurationOptions = Guard.CheckNullArgument(configurationOptions, nameof(configurationOptions));
            _textWriter = textWriter;
        }

        /// <inheritdoc />
        public async Task<IInternalDisposableConnectionMultiplexer> CreateAsync()
        {
            IConnectionMultiplexer connectionMultiplexer;

            if (_configuration != null)
            {
                connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(_configuration, _textWriter).ConfigureAwait(continueOnCapturedContext: false);
            }
            else
            {
                connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(_configurationOptions, _textWriter).ConfigureAwait(continueOnCapturedContext: false);
            }

            return new InternalDisposableConnectionMultiplexer(connectionMultiplexer);
        }

        private readonly string _configuration;
        private readonly TextWriter _textWriter;
        private readonly ConfigurationOptions _configurationOptions;
    }
}
