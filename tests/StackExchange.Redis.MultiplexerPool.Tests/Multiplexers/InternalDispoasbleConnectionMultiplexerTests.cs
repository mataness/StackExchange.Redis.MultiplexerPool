using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using StackExchange.Redis.MultiplexerPool.Multiplexers;
using TestStack.BDDfy;

namespace StackExchange.Redis.MultiplexerPool.Tests.Multiplexers
{
    /// <summary>
    /// Test class for <see cref="InternalDisposableConnectionMultiplexer"/>
    /// </summary>
    [TestFixture]
    public class InternalDispoasbleConnectionMultiplexerTests
    {
        [SetUp]
        public void SetUp()
        {
            _exceptionThrown = null;
            _connectionMultiplexerMock = new Mock<IConnectionMultiplexer>();

            _internalDisposableConnectionMultiplexer = new InternalDisposableConnectionMultiplexer(_connectionMultiplexerMock.Object);
        }

        [Test]
        public void Throws_InvalidOperationException_when_trying_to_dispose_the_connection()
        {
            this.When(_ => Disposing_the_connection())
                .Then(_ => InvalidOperationException_was_thrown())
                .BDDfy();
        }
        [Test]
        public void Throws_InvalidOperationException_when_trying_to_close_the_connection()
        {
            this.When(_ => Closing_the_connection())
                .Then(_ => InvalidOperationException_was_thrown())
                .BDDfy();
        }

        [Test]
        public void Throws_InvalidOperationException_when_trying_to_close_the_connection_asynchronously()
        {
            this.When(_ => Closing_the_connection_asynchronously())
                .Then(_ => InvalidOperationException_was_thrown())
                .BDDfy();
        }

        [Test]
        public void Closes_the_connection_when_safely_closing_asynchronously()
        {
            this.When(_ => Safely_closing_the_connection_asynchronously())
                .Then(_ => The_underlying_connection_was_closed_asynchronously())
                .BDDfy();
        }


        [Test]
        public void Closes_the_connection_when_safely_closing()
        {
            this.When(_ => Safely_closing_the_connection())
                .Then(_ => The_underlying_connection_was_closed())
                .BDDfy();
        }

        private void Safely_closing_the_connection_asynchronously()
            => _internalDisposableConnectionMultiplexer.SafeCloseAsync();

        private void Safely_closing_the_connection()
            => _internalDisposableConnectionMultiplexer.SafeClose();

        private void Disposing_the_connection()
        {
            try
            {
                _internalDisposableConnectionMultiplexer.Dispose();
            }
            catch (Exception ex)
            {
                _exceptionThrown = ex;
            }
        }

        private void Closing_the_connection()
        {
            try
            {
                _internalDisposableConnectionMultiplexer.Close();
            }
            catch (Exception ex)
            {
                _exceptionThrown = ex;
            }
        }

        private void Closing_the_connection_asynchronously()
        {
            try
            {
                _internalDisposableConnectionMultiplexer.CloseAsync();
            }
            catch (Exception ex)
            {
                _exceptionThrown = ex;
            }
        }

        private void The_underlying_connection_was_closed()
            => _connectionMultiplexerMock.Verify(m => m.Close(It.IsAny<bool>()), Times.Once);

        private void The_underlying_connection_was_closed_asynchronously()
            => _connectionMultiplexerMock.Verify(m => m.CloseAsync(It.IsAny<bool>()), Times.Once);

        private void InvalidOperationException_was_thrown()
            => _exceptionThrown.Should().BeOfType<InvalidOperationException>();

        private Mock<IConnectionMultiplexer> _connectionMultiplexerMock;
        private InternalDisposableConnectionMultiplexer _internalDisposableConnectionMultiplexer;
        private Exception _exceptionThrown;
    }
}
