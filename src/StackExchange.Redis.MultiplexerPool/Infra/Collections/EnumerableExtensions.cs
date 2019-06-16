using System;
using System.Collections.Generic;
using StackExchange.Redis.MultiplexerPool.Infra.Common;

namespace StackExchange.Redis.MultiplexerPool.Infra.Collections
{
    /// <summary>
    /// Extension methods for <see cref="IEnumerable{T}"/>
    /// </summary>
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// Gets the minimal element in <paramref name="source"/> by comparing according to a computed value with default value comparer.
        /// </summary>
        /// <param name="source">
        /// The source collection.
        /// </param>
        /// <param name="selector">
        /// A selector that calculates a value for an element in <paramref name="source"/> that the comparison should be done by.
        /// Will receive null arguments if there are nulls in <paramref name="source"/>.
        /// </param>
        /// <param name="comparer">The comparer to use for comparing between the <typeparamref name="TKey"/></param>
        /// <typeparam name="TSource">
        /// Type of elements in <paramref name="source"/>.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// Type of value to compare elements by.
        /// </typeparam>
        /// <returns>
        /// The minimal element in <paramref name="source"/>.
        /// </returns>
        public static TSource MinBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> selector,
            IComparer<TKey> comparer = null)
        {
            Guard.CheckNullArgument(source, nameof(source));
            Guard.CheckNullArgument(selector, nameof(selector));


            comparer = comparer ?? Comparer<TKey>.Default;

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var min = sourceIterator.Current;
                var minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
                return min;
            }
        }
    }
}
