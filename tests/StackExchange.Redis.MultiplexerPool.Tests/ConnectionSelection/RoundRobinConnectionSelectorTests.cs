using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using StackExchange.Redis.MultiplexerPool.ConnectionSelection;
using TestStack.BDDfy;

namespace StackExchange.Redis.MultiplexerPool.Tests.ConnectionSelection
{
    /// <summary>
    /// Test class for <see cref="RoundRobinConnectionSelector"/>
    /// </summary>
    [TestFixture]
    public class RoundRobinConnectionSelectorTests
    {
        [SetUp]
        public void SetUp()
        {
            _roundRobinConnectionSelector = new RoundRobinConnectionSelector();
            _connections = null;
            _selectedConnection = null;
        }

        [Test]
        public void Selects_the_connection_in_round_robin_strategy()
        {
            this.Given(_ => A_list_of_connections_in_size_of(3))
                .When(_ => Selecting_a_connection())
                .Then(_ => The_selected_connection_index_is(0))
                .When(_ => Selecting_a_connection())
                .Then(_ => The_selected_connection_index_is(1))
                .When(_ => Selecting_a_connection())
                .Then(_ => The_selected_connection_index_is(2))
                .When(_ => Selecting_a_connection())
                .Then(_ => The_selected_connection_index_is(0))
                .BDDfy();
        }

        [Test]
        public void Always_returns_the_same_connection_when_the_connection_list_is_in_size_of_one()
        {
            this.Given(_ => A_list_of_connections_in_size_of(1))
                .When(_ => Selecting_a_connection())
                .Then(_ => The_selected_connection_index_is(0))
                .When(_ => Selecting_a_connection())
                .Then(_ => The_selected_connection_index_is(0))
                .When(_ => Selecting_a_connection())
                .Then(_ => The_selected_connection_index_is(0))
                .BDDfy();
        }

        private void A_list_of_connections_in_size_of(int size)
            => _connections = Enumerable.Range(0, size)
                .Select(_ => new Mock<IReconnectableConnectionMultiplexer>().Object)
                .ToList();

        private void Selecting_a_connection()
            => _selectedConnection = _roundRobinConnectionSelector.Select(_connections);

        private void The_selected_connection_index_is(int expectedIndex)
            => _selectedConnection.Should().BeSameAs(_connections[expectedIndex]);

        private List<IReconnectableConnectionMultiplexer> _connections;
        private IReconnectableConnectionMultiplexer _selectedConnection;
        private RoundRobinConnectionSelector _roundRobinConnectionSelector;
    }
}
