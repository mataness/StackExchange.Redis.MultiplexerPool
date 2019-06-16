using System.Collections.Generic;
using StackExchange.Redis.MultiplexerPool.Infra.Collections;

namespace StackExchange.Redis.MultiplexerPool.ConnectionSelection
{
    /// <summary>
    /// Implements <see cref="IConnectionSelector"/>.
    /// The implementation selects the connection by its amount of outstanding operations.
    /// The connection with the minimal outstanding connections is selected.
    /// The amount of outstanding operations of every connection is retrieved using the <see cref="IConnectionMultiplexer.GetCounters"/> method. 
    /// </summary>
    internal class LoadBasedConnectionSelector : IConnectionSelector
    {
        /// <inheritdoc />
        public IReconnectableConnectionMultiplexer Select(IReadOnlyList<IReconnectableConnectionMultiplexer> establishedConnections)
            => establishedConnections.MinBy(connection => connection.Connection.GetCounters().TotalOutstanding);
    }
}
