using System.Collections.Generic;

namespace StackExchange.Redis.MultiplexerPool.ConnectionSelection
{
    /// <summary>
    /// A contract for selecting a <see cref="IReconnectableConnectionMultiplexer"/> from a given collection of established connections.
    /// Each implementation defines its own strategy for selecting a connection.
    /// The implementation must be thread safe
    /// </summary>
    internal interface IConnectionSelector
    {
        /// <summary>
        /// Selects a connection from the given list and returns it
        /// </summary>
        /// <param name="establishedConnections">The list of connections to choose from</param>
        /// <returns>The selected connection</returns>
        IReconnectableConnectionMultiplexer Select(IReadOnlyList<IReconnectableConnectionMultiplexer> establishedConnections);
    }
}
