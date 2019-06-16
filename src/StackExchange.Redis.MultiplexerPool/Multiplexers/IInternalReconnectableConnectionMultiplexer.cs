namespace StackExchange.Redis.MultiplexerPool.Multiplexers
{
    /// <summary>
    /// An extended contract of <see cref="IInternalReconnectableConnectionMultiplexer"/> which overrides the <see cref="IReconnectableConnectionMultiplexer.Connection"/> by returning
    /// <see cref="IInternalDisposableConnectionMultiplexer"/>.
    /// </summary>
    internal interface IInternalReconnectableConnectionMultiplexer : IReconnectableConnectionMultiplexer
    {
        new IInternalDisposableConnectionMultiplexer Connection { get; }
    }
}
