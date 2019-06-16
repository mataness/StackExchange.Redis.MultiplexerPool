using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using StackExchange.Redis.MultiplexerPool.ConnectionSelection;
using StackExchange.Redis.MultiplexerPool.MultiplexerPools;
using StackExchange.Redis.MultiplexerPool.Multiplexers;
using TestStack.BDDfy;

namespace StackExchange.Redis.MultiplexerPool.Tests.MultiplexerPools
{
    /// <summary>
    /// Test class for <see cref="ConnectionMultiplexerPool"/>
    /// </summary>
    [TestFixture]
    public class ConnectionMultiplexerPoolTests
    {
        [SetUp]
        public void SetUp()
        {
            _connectionFactoryMock = null;
            _connectionPool = null;
            _exceptionThrown = null;
            _retrievedConnection = null;
            _connectionSelectorMock = null;
            _createdConnectionsMocks = new List<Mock<IInternalDisposableConnectionMultiplexer>>();
        }

        [Test]
        public void Returns_the_connection_chosen_by_the_selector_when_all_connections_has_been_established()
        {
            this.Given(_ => A_connection_pool_of_size(1))
                .When(_ => Getting_a_connection())
                .When(_ => Getting_a_connection())
                .Then(_ => The_return_connection_was_chosen_by_the_selector())
                .BDDfy();
        }

        [Test]
        public void Passing_the_allowCommandsToComplete_as_true_when_disposing()
        {
            this.Given(_ => A_connection_pool())
                .When(_ => Disposing())
                .Then(_ => All_open_connections_has_been_closed_with_allowCommandsToComplete_set_to(true))
                .BDDfy();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Passing_the_given_allowCommandsToComplete_when_closing_all(bool value)
        {
            this.Given(_ => A_connection_pool())
                .When(_ => Closing_all_connections_with_allowCommandsToComplete_set_to(value))
                .Then(_ => All_open_connections_has_been_closed_with_allowCommandsToComplete_set_to(value))
                .BDDfy();
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        public void Returns_the_pool_size(int poolSize)
        {
            this.Given(_ => A_connection_pool_of_size(poolSize))
                .When(_ => Getting_the_pool_size())
                .Then(_ => The_returned_pool_size_value_is(poolSize))
                .BDDfy();
        }

        [Test]
        public void Throws_object_disposed_exception_when_trying_to_get_a_connection_after_closing_all_connections()
        {
            this.Given(_ => A_connection_pool_of_size(10))
                .When(_ => Closing_all_connections())
                .And(_ => Getting_a_connection())
                .Then(_ => ObjectDisposedException_was_thrown())
                .BDDfy();
        }

        [Test]
        public void Throws_object_disposed_exception_when_trying_to_get_a_connection_after_disposing()
        {
            this.Given(_ => A_connection_pool_of_size(10))
                .When(_ => Disposing_the_pool())
                .And(_ => Getting_a_connection())
                .Then(_ => ObjectDisposedException_was_thrown())
                .BDDfy();
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        public void Creates_a_new_connection_when_pool_size_hasnt_reached(int poolSize)
        {
            this.Given(_ => A_connection_pool_of_size(poolSize))
                .When(_ => Getting_a_connection())
                .Then(_ => The_amount_of_created_connections_is(1))
                .When(_ => Getting_a_connection())
                .Then(_ => The_amount_of_created_connections_is(Math.Min(poolSize, 2)))
                .BDDfy();
        }

        private void A_connection_pool()
            => A_connection_pool_of_size(10);

        private void Disposing() => _connectionPool.Dispose();

        private void A_connection_pool_of_size(int size)
        {
            _connectionFactoryMock = CreateConnectionFactoryMock();
            _connectionSelectorMock = new Mock<IConnectionSelector>();

            var connectionToReturn = new Mock<IReconnectableConnectionMultiplexer>().Object;

            _connectionSelectorMock
                .Setup(m => m.Select(It.IsAny<IReadOnlyList<IReconnectableConnectionMultiplexer>>()))
                .Returns(connectionToReturn);

            _connectionPool = new ConnectionMultiplexerPool(size, _connectionFactoryMock.Object, _connectionSelectorMock.Object);
        }

        private Task Closing_all_connections_with_allowCommandsToComplete_set_to(bool value)
            => _connectionPool.CloseAllAsync(value);

        private Task Closing_all_connections()
            => _connectionPool.CloseAllAsync();

        private void Disposing_the_pool()
            => _connectionPool.Dispose();

        private void Getting_the_pool_size()
            => _retrievedPoolSize = _connectionPool.PoolSize;

        private async Task Getting_a_connection()
        {
            try
            {
                _retrievedConnection = await _connectionPool.GetAsync();
            }
            catch (Exception ex)
            {
                _exceptionThrown = ex;
            }
        }

        private void The_amount_of_created_connections_is(int expectedCountOfCreatedConnections)
            => _connectionFactoryMock.Verify(m => m.CreateAsync(It.IsAny<int>()), Times.Exactly(expectedCountOfCreatedConnections));

        private void ObjectDisposedException_was_thrown()
            => _exceptionThrown.Should().BeOfType<ObjectDisposedException>();

        private void The_returned_pool_size_value_is(int expectedSize)
            => _retrievedPoolSize.Should().Be(expectedSize);

        private void All_open_connections_has_been_closed_with_allowCommandsToComplete_set_to(bool expectedValue)
            => _createdConnectionsMocks.ForEach(m => m.Verify(mock => mock.SafeClose(expectedValue), Times.Once));

        private void The_return_connection_was_chosen_by_the_selector()
            => _retrievedConnection.Should().BeSameAs(_connectionSelectorMock.Object.Select(null));

        private Mock<IReconnectableConnectionMultiplexerFactory> CreateConnectionFactoryMock()
        {
            var connectionFactoryMock = new Mock<IReconnectableConnectionMultiplexerFactory>();

            connectionFactoryMock
                .Setup(m => m.CreateAsync(It.IsAny<int>()))
                .Returns(() =>
                {
                    var reconnectableMock = new Mock<IInternalReconnectableConnectionMultiplexer>();
                    var connectionMock = new Mock<IInternalDisposableConnectionMultiplexer>();
                    reconnectableMock.Setup(m => m.Connection).Returns(connectionMock.Object);

                    _createdConnectionsMocks.Add(connectionMock);

                    return Task.FromResult(reconnectableMock.Object);
                });

            return connectionFactoryMock;
        }

        private List<Mock<IInternalDisposableConnectionMultiplexer>> _createdConnectionsMocks;
        private ConnectionMultiplexerPool _connectionPool;
        private Mock<IConnectionSelector> _connectionSelectorMock;
        private Mock<IReconnectableConnectionMultiplexerFactory> _connectionFactoryMock;
        private IReconnectableConnectionMultiplexer _retrievedConnection;
        private Exception _exceptionThrown;
        private int _retrievedPoolSize;
    }
}
