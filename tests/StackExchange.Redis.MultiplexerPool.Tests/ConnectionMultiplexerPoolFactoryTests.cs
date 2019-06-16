using System;
using System.Reflection;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using StackExchange.Redis.MultiplexerPool.ConnectionSelection;
using StackExchange.Redis.MultiplexerPool.MultiplexerPools;
using TestStack.BDDfy;

namespace StackExchange.Redis.MultiplexerPool.Tests
{
    /// <summary>
    /// Test class for <see cref="ConnectionMultiplexerPoolFactory"/>
    /// </summary>
    [TestFixture]
    public class ConnectionMultiplexerPoolFactoryTests
    {
        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _fixture.Customizations.Add(new RandomNumericSequenceGenerator(1, 100));
            _expectedSize = 0;
            _createdPool = null;
        }

        [Test]
        [TestCase(ConnectionSelectionStrategy.LeastLoaded, typeof(LoadBasedConnectionSelector))]
        [TestCase(ConnectionSelectionStrategy.RoundRobin, typeof(RoundRobinConnectionSelector))]
        public void Creates_a_connection_pool_based_on_the_given_strategy_and_with_the_given_size(ConnectionSelectionStrategy strategy, Type expectedConnectionSelectorType)
        {
            this.When(_ => Creating_a_connection_pool_with_strategy_and_size(strategy, _fixture.Create<int>()))
                .Then(_ => The_type_of_the_created_connection_selector_is(expectedConnectionSelectorType))
                .And(_ => The_size_of_the_pool_is(_expectedSize))
                .BDDfy();
        }

        private void Creating_a_connection_pool_with_strategy_and_size(ConnectionSelectionStrategy strategy, int poolSize)
        {
            _expectedSize = poolSize;

            /* Providing a random string which acts as a mock for the connection string,
             will not result in exception as the connections are lazily created */
            _createdPool = ConnectionMultiplexerPoolFactory.Create(
                poolSize,
                _fixture.Create<string>() ,
                connectionSelectionStrategy: strategy) as ConnectionMultiplexerPool;
        }

        private void The_type_of_the_created_connection_selector_is(Type expectedConnectionPoolType)
        {

            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.NonPublic;

            FieldInfo field = _createdPool.GetType().GetField("_connectionSelector", bindFlags);

            field.GetValue(_createdPool).Should().BeOfType(expectedConnectionPoolType);
        }

        private void The_size_of_the_pool_is(int expectedSize)
            => AssertionExtensions.Should((int) _createdPool.PoolSize).Be(expectedSize);

        private Fixture _fixture;
        private ConnectionMultiplexerPool _createdPool;
        private int _expectedSize;
    }
}
