using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using StackExchange.Redis.MultiplexerPool.Multiplexers;
using TestStack.BDDfy;

namespace StackExchange.Redis.MultiplexerPool.Tests.Multiplexers
{
    /// <summary>
    /// Test class for <see cref="ReconnectableConnectionMultiplexer"/>
    /// </summary>
    [TestFixture]
    public class ReconnectableConnectionMultiplexerTests
    {
        [SetUp]
        public void SetUp()
        {
            _methodCallsOrdered = new List<string>();

            _createdConnectionMultiplexer = new Mock<IInternalDisposableConnectionMultiplexer>();

            _connectionMultiplexerMock = new Mock<IInternalDisposableConnectionMultiplexer>();
            _factoryMock = new Mock<IConnectionMultiplexerFactory>();

            _factoryMock
                .Setup(m => m.CreateAsync())
                .Callback(() => _methodCallsOrdered.Add(nameof(IConnectionMultiplexerFactory.CreateAsync)))
                .ReturnsAsync(_createdConnectionMultiplexer.Object);

            _connectionMultiplexerMock
                .Setup(m => m.SafeCloseAsync(It.IsAny<bool>()))
                .Callback(() => _methodCallsOrdered.Add(nameof(IInternalDisposableConnectionMultiplexer.SafeCloseAsync)))
                .Returns(Task.CompletedTask);

            _reconnectableConnectionMultiplexer = new ReconnectableConnectionMultiplexer(0, _connectionMultiplexerMock.Object, _factoryMock.Object);
        }

        [Test]
        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        [TestCase(false, true)]
        public void Closes_previous_connection_only_after_a_new_one_has_been_created_when_reconnecting(bool allowCommandsToComplete, bool fireAndForgetOnClose)
        {
            this.When(_ => Reconnecting(allowCommandsToComplete, fireAndForgetOnClose))
                .Then(_ => The_connection_time_has_been_updated_to_utc_now())
                .And(_ => A_new_connection_has_been_created())
                .And(_ => After_that_the_previous_connection_has_been_closed())
                .BDDfy();
        }

        private void The_connection_time_has_been_updated_to_utc_now()
            => _reconnectableConnectionMultiplexer.ConnectionTimeUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMilliseconds(100));

        private Task Reconnecting(bool allowCommandsToComplete, bool fireAndForgetOnClose)
            => _reconnectableConnectionMultiplexer.ReconnectAsync(allowCommandsToComplete, fireAndForgetOnClose);

        private void A_new_connection_has_been_created()
            => _factoryMock.Verify(m => m.CreateAsync(), Times.Once);

        private void After_that_the_previous_connection_has_been_closed()
        {
            _methodCallsOrdered.Should().HaveCount(2);
            _methodCallsOrdered[0].Should().Be(nameof(IConnectionMultiplexerFactory.CreateAsync));
            _methodCallsOrdered[1].Should().Be(nameof(IInternalDisposableConnectionMultiplexer.SafeCloseAsync));
        }



        private Mock<IInternalDisposableConnectionMultiplexer> _connectionMultiplexerMock;
        private ReconnectableConnectionMultiplexer _reconnectableConnectionMultiplexer;
        private Mock<IInternalDisposableConnectionMultiplexer> _createdConnectionMultiplexer;
        private Mock<IConnectionMultiplexerFactory> _factoryMock;

        private List<string> _methodCallsOrdered;
    }
}
