using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using StackExchange.Redis.Maintenance;
using StackExchange.Redis.MultiplexerPool.Infra.Common;
using StackExchange.Redis.Profiling;

namespace StackExchange.Redis.MultiplexerPool.Multiplexers
{
    /// <summary>
    /// Implements <see cref="IInternalDisposableConnectionMultiplexer"/> in a decorator style pattern by decorating a given <see cref="IConnectionMultiplexer"/>.
    /// The implementation passes the calls to the decorated <see cref="IConnectionMultiplexer"/>.
    /// Calls to <see cref="Dispose"/>, <see cref="Close"/> or <see cref="CloseAsync"/> will result in <see cref="InvalidOperationException"/> being thrown.
    /// </summary>
    internal class InternalDisposableConnectionMultiplexer : IInternalDisposableConnectionMultiplexer
    {
        public InternalDisposableConnectionMultiplexer(IConnectionMultiplexer wrappedConnectionMultiplexer)
        {
            _wrappedConnectionMultiplexer = Guard.CheckNullArgument(wrappedConnectionMultiplexer, nameof(wrappedConnectionMultiplexer));
        }

        /// <inheritdoc />
        public void Dispose()
            => throw CreateDisposeNotAllowedException();

        /// <inheritdoc />
        public ValueTask DisposeAsync()
            => new ValueTask(Task.FromException(CreateDisposeNotAllowedException()));

        /// <inheritdoc />
        public Task SafeCloseAsync(bool allowCommandsToComplete = true)
            => _wrappedConnectionMultiplexer.CloseAsync(allowCommandsToComplete);

        /// <inheritdoc />
        public void SafeClose(bool allowCommandsToComplete = true)
            => _wrappedConnectionMultiplexer.Close();

        /// <inheritdoc />
        public void RegisterProfiler(Func<ProfilingSession> profilingSessionProvider)
            => _wrappedConnectionMultiplexer.RegisterProfiler(profilingSessionProvider);

        /// <inheritdoc />
        public ServerCounters GetCounters()
            => _wrappedConnectionMultiplexer.GetCounters();

        /// <inheritdoc />
        public EndPoint[] GetEndPoints(bool configuredOnly = false)
            => _wrappedConnectionMultiplexer.GetEndPoints(configuredOnly);

        /// <inheritdoc />
        public void Wait(Task task)
            => _wrappedConnectionMultiplexer.Wait(task);

        /// <inheritdoc />
        public T Wait<T>(Task<T> task)
            => _wrappedConnectionMultiplexer.Wait(task);

        /// <inheritdoc />
        public void WaitAll(params Task[] tasks)
            => _wrappedConnectionMultiplexer.WaitAll(tasks);

        /// <inheritdoc />
        public int HashSlot(RedisKey key)
            => _wrappedConnectionMultiplexer.HashSlot(key);

        /// <inheritdoc />
        public ISubscriber GetSubscriber(object asyncState = null)
            => _wrappedConnectionMultiplexer.GetSubscriber(asyncState);


        /// <inheritdoc />
        public IDatabase GetDatabase(int db = -1, object asyncState = null)
            => _wrappedConnectionMultiplexer.GetDatabase(db, asyncState);


        /// <inheritdoc />
        public IServer GetServer(string host, int port, object asyncState = null)
            => _wrappedConnectionMultiplexer.GetServer(host, port, asyncState);


        /// <inheritdoc />
        public IServer GetServer(string hostAndPort, object asyncState = null)
            => _wrappedConnectionMultiplexer.GetServer(hostAndPort, asyncState);


        /// <inheritdoc />
        public IServer GetServer(IPAddress host, int port)
            => _wrappedConnectionMultiplexer.GetServer(host, port);


        /// <inheritdoc />
        public IServer GetServer(EndPoint endpoint, object asyncState = null)
            => _wrappedConnectionMultiplexer.GetServer(endpoint, asyncState);

        /// <inheritdoc />
        public IServer[] GetServers()
            => _wrappedConnectionMultiplexer.GetServers();


        /// <inheritdoc />
        public Task<bool> ConfigureAsync(TextWriter log = null)
            => _wrappedConnectionMultiplexer.ConfigureAsync(log);


        /// <inheritdoc />
        public bool Configure(TextWriter log = null)
            => _wrappedConnectionMultiplexer.Configure(log);


        /// <inheritdoc />
        public string GetStatus()
            => _wrappedConnectionMultiplexer.GetStatus();


        /// <inheritdoc />
        public void GetStatus(TextWriter log)
            => _wrappedConnectionMultiplexer.GetStatus(log);


        /// <inheritdoc />
        public void Close(bool allowCommandsToComplete = true)
            => throw CreateDisposeNotAllowedException();


        /// <inheritdoc />
        public Task CloseAsync(bool allowCommandsToComplete = true)
            => throw CreateDisposeNotAllowedException();


        /// <inheritdoc />
        public string GetStormLog()
            => _wrappedConnectionMultiplexer.GetStormLog();


        /// <inheritdoc />
        public void ResetStormLog()
            => _wrappedConnectionMultiplexer.ResetStormLog();


        /// <inheritdoc />
        public long PublishReconfigure(CommandFlags flags = CommandFlags.None)
            => _wrappedConnectionMultiplexer.PublishReconfigure(flags);


        /// <inheritdoc />
        public Task<long> PublishReconfigureAsync(CommandFlags flags = CommandFlags.None)
            => _wrappedConnectionMultiplexer.PublishReconfigureAsync(flags);


        /// <inheritdoc />
        public int GetHashSlot(RedisKey key)
            => _wrappedConnectionMultiplexer.GetHashSlot(key);


        /// <inheritdoc />
        public void ExportConfiguration(Stream destination, ExportOptions options = ExportOptions.All)
            => _wrappedConnectionMultiplexer.ExportConfiguration(destination, options);

        /// <inheritdoc />
        public void AddLibraryNameSuffix(string suffix)
            => _wrappedConnectionMultiplexer.AddLibraryNameSuffix(suffix);


        /// <inheritdoc />
        public string ClientName => _wrappedConnectionMultiplexer.ClientName;

        /// <inheritdoc />
        public string Configuration => _wrappedConnectionMultiplexer.Configuration;

        /// <inheritdoc />
        public int TimeoutMilliseconds => _wrappedConnectionMultiplexer.TimeoutMilliseconds;

        /// <inheritdoc />
        public long OperationCount => _wrappedConnectionMultiplexer.OperationCount;

        /// <inheritdoc />
        public bool PreserveAsyncOrder
        {
#pragma warning disable CS0618 // Type or member is obsolete
            get => _wrappedConnectionMultiplexer.PreserveAsyncOrder;

            set => _wrappedConnectionMultiplexer.PreserveAsyncOrder = value;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <inheritdoc />
        public bool IsConnected => _wrappedConnectionMultiplexer.IsConnected;

        /// <inheritdoc />
        public bool IsConnecting => _wrappedConnectionMultiplexer.IsConnecting;

        /// <inheritdoc />
        [Obsolete("Obsolete")]
        public bool IncludeDetailInExceptions
        {
            get => _wrappedConnectionMultiplexer.IncludeDetailInExceptions;
            set => _wrappedConnectionMultiplexer.IncludeDetailInExceptions = value;
        }

        /// <inheritdoc />
        public int StormLogThreshold
        {
            get => _wrappedConnectionMultiplexer.StormLogThreshold;
            set => _wrappedConnectionMultiplexer.StormLogThreshold = value;
        }

        /// <inheritdoc />
        public event EventHandler<RedisErrorEventArgs> ErrorMessage
        {
            add => _wrappedConnectionMultiplexer.ErrorMessage += value;
            remove => _wrappedConnectionMultiplexer.ErrorMessage -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ConnectionFailedEventArgs> ConnectionFailed
        {
            add => _wrappedConnectionMultiplexer.ConnectionFailed += value;
            remove => _wrappedConnectionMultiplexer.ConnectionFailed -= value;
        }

        /// <inheritdoc />
        public event EventHandler<InternalErrorEventArgs> InternalError
        {
            add => _wrappedConnectionMultiplexer.InternalError += value;
            remove => _wrappedConnectionMultiplexer.InternalError -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ConnectionFailedEventArgs> ConnectionRestored
        {
            add => _wrappedConnectionMultiplexer.ConnectionRestored += value;
            remove => _wrappedConnectionMultiplexer.ConnectionRestored -= value;
        }

        /// <inheritdoc />
        public event EventHandler<EndPointEventArgs> ConfigurationChanged
        {
            add => _wrappedConnectionMultiplexer.ConfigurationChanged += value;
            remove => _wrappedConnectionMultiplexer.ConfigurationChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<EndPointEventArgs> ConfigurationChangedBroadcast
        {
            add => _wrappedConnectionMultiplexer.ConfigurationChangedBroadcast += value;
            remove => _wrappedConnectionMultiplexer.ConfigurationChangedBroadcast -= value;
        }

        public event EventHandler<ServerMaintenanceEvent> ServerMaintenanceEvent
        {
            add => _wrappedConnectionMultiplexer.ServerMaintenanceEvent += value;
            remove => _wrappedConnectionMultiplexer.ServerMaintenanceEvent -= value;
        }

        /// <inheritdoc />
        public event EventHandler<HashSlotMovedEventArgs> HashSlotMoved
        {
            add => _wrappedConnectionMultiplexer.HashSlotMoved += value;
            remove => _wrappedConnectionMultiplexer.HashSlotMoved -= value;
        }

        private static InvalidOperationException CreateDisposeNotAllowedException()
            => new InvalidOperationException($"Disposing or closing the connection of '{nameof(IConnectionMultiplexer)}' is not allowed, please dispose / close the '{nameof(IConnectionMultiplexerPool)}' instead");

        private readonly IConnectionMultiplexer _wrappedConnectionMultiplexer;
    }
}
