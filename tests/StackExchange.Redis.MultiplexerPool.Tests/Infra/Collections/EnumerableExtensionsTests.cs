using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using TestStack.BDDfy;

namespace StackExchange.Redis.MultiplexerPool.Tests.Infra.Collections
{
    /// <summary>
    /// Test class for <see cref="MultiplexerPool.Infra.Collections.EnumerableExtensions"/>
    /// </summary>
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [SetUp]
        public void SetUp()
        {
            _returnedMinimum = null;
            _collection = null;
            _fixture = new Fixture();
        }

        [Test]
        [TestCase(10)]
        [TestCase(1)]
        [TestCase(2)]
        public void Returns_the_item_with_the_minimum_value(int collectionSize)
        {
            this.Given(_ => A_collection_of_numbers_with_size(collectionSize))
                .When(_ => Getting_the_minimum_number_in_the_collection())
                .Then(_ => The_minimal_number_in_the_collection_was_returned())
                .BDDfy();
        }

        [Test]
        public void Throws_InvalidOperationException_when_the_collection_is_empty()
        {
            this.Given(_ => An_empty_collection_of_numbers())
                .When(_ => Getting_the_minimum_number_in_the_collection())
                .Then(_ => InvalidOperationException_was_thrown())
                .BDDfy();
        }

        private void An_empty_collection_of_numbers()
            => _collection = new List<int>();

        private void A_collection_of_numbers_with_size(int collectionSize)
            => _collection = _fixture.CreateMany<int>(collectionSize).ToList();

        private void Getting_the_minimum_number_in_the_collection()
        {
            try
            {
                _returnedMinimum = MultiplexerPool.Infra.Collections.EnumerableExtensions.MinBy(_collection, num => num);
            }
            catch (Exception ex)
            {
                _exceptionThrown = ex;
            }
        }

        private void The_minimal_number_in_the_collection_was_returned()
            => _returnedMinimum.Should().Be(_collection.Min());

        private void InvalidOperationException_was_thrown()
            => _exceptionThrown.Should().BeOfType<InvalidOperationException>();


        private List<int> _collection;
        private int? _returnedMinimum;
        private Exception _exceptionThrown;
        private Fixture _fixture;
    }
}
