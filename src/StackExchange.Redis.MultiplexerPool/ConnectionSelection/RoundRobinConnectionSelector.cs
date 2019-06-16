using System.Collections.Generic;
using System.Threading;

namespace StackExchange.Redis.MultiplexerPool.ConnectionSelection
{
    /// <summary>
    /// Implements <see cref="IConnectionSelector"/>.
    /// The implementation selects the connection in a round robin strategy in a thread safe manner.
    /// </summary>
    public class RoundRobinConnectionSelector : IConnectionSelector
    {
        internal RoundRobinConnectionSelector()
        {
            // Starting from -1 since we always get the next index in Get
            _lastConnectionChosen = -1;
        }

        /// <inheritdoc />
        public IReconnectableConnectionMultiplexer Select(IReadOnlyList<IReconnectableConnectionMultiplexer> establishedConnections)
        {
            var nextNumber = unchecked((uint)Interlocked.Increment(ref _lastConnectionChosen));
            var nextIdx = (int)(nextNumber % establishedConnections.Count);

            return establishedConnections[nextIdx];
        }

        private int _lastConnectionChosen;
    }
}
