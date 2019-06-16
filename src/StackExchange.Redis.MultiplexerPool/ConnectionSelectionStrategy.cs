namespace StackExchange.Redis.MultiplexerPool
{
    /// <summary>
    /// Possible strategies for selecting the <see cref="IConnectionMultiplexer"/> returned for every call to <see cref="IConnectionMultiplexerPool.GetAsync"/>
    /// </summary>
    public enum ConnectionSelectionStrategy
    {
        /// <summary>
        /// Every call to <see cref="IConnectionMultiplexerPool.GetAsync"/> will return the next connection in the pool in a round robin manner.
        /// </summary>
        RoundRobin,

        /// <summary>
        /// Every call to <see cref="IConnectionMultiplexerPool.GetAsync"/> will return the least loaded <see cref="IConnectionMultiplexer"/>.
        /// The load of every connection is defined by it's <see cref="ServerCounters.TotalOutstanding"/>.
        /// For more info refer to https://github.com/StackExchange/StackExchange.Redis/issues/512 .
        /// </summary>
        LeastLoaded
    }
}