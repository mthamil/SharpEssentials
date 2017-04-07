﻿// Sharp Essentials
// Copyright 2017 Matthew Hamilton - matthamilton@live.com
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SharpEssentials.Collections
{
    /// <summary>
    /// Contains extension methods pertaining to enumerables.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Determines whether an enumerable contains all items of another enumerable.
        /// This does not test for proper subsets, so if the compared enumerables 
        /// contain exactly the same items, true is returned.
        /// </summary>
        /// <param name="subset">The supposed subset of items</param>
        /// <param name="superset">The supposed superset of items</param>
        /// <returns>Whether superset contains all elements of the enumerable</returns>
        public static bool IsSubsetOf<T>(this IEnumerable<T> subset, IEnumerable<T> superset)
        {
            // Optimize for ISet which already contains a method to perform
            // this operation.
            if (subset is ISet<T> subsetSet)
                return subsetSet.IsSubsetOf(superset);

            // If subtracting the superset from the subset does not remove all items, then
            // the superset does not actually contain everything.
            var subtractedItems = subset.Except(superset);
            return !subtractedItems.Any();
        }

        /// <summary>
        /// Groups an enumerable of items into a sequence of <paramref name="sliceSize"/>-sized collections.
        /// However, if the number of items remaining is fewer than the <paramref name="sliceSize"/>, the last
        /// slice will contain just the remaining items.
        /// </summary>
        /// <param name="sourceItems">The items to slice</param>
        /// <param name="sliceSize">The number of items per slice</param>
        /// <returns>An enumerable of items grouped into <paramref name="sliceSize"/> number of items</returns>
        public static IEnumerable<IReadOnlyCollection<T>> Slices<T>(this IEnumerable<T> sourceItems, int sliceSize)
        {
            if (sourceItems == null)
                throw new ArgumentNullException(nameof(sourceItems));

            if (sliceSize < 1)
                return Enumerable.Empty<IReadOnlyCollection<T>>();

            return new SliceEnumerable<T>(sourceItems, sliceSize);
        }

        /// <summary>
        /// Generates an IEnumerable from an IEnumerator.
        /// </summary>
        /// <param name="enumerator">The enumerator to convert</param>
        /// <returns>An enumerable for the enumerator</returns>
        /// <remarks>Borrowed from Igor Ostrovsky: 
        /// http://igoro.com/archive/extended-linq-additional-operators-for-linq-to-objects/
        /// </remarks>
        [DebuggerStepThrough]
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> enumerator)
        {
            if (enumerator == null)
                throw new ArgumentNullException(nameof(enumerator));

            IEnumerable<T> Impl()
            {
                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }

            return Impl();
        }

        /// <summary>
        /// Generates an IEnumerable from a single value.
        /// </summary>
        /// <param name="value">The value to create an enumerable for</param>
        /// <returns>An enumerable for the single value</returns>
        [DebuggerStepThrough]
        public static IEnumerable<T> ToEnumerable<T>(this T value)
        {
            yield return value;
        }

        /// <summary>
        /// Returns the item from an enumerable that has the maximum value according
        /// to a key.  The default comparer is used.
        /// </summary>
        /// <typeparam name="TSource">The type of items in the source enumerable</typeparam>
        /// <typeparam name="TKey">The type of value being compared</typeparam>
        /// <param name="source">The source enumerable</param>
        /// <param name="selector">Determines the value from each item to use for comparison</param>
        /// <returns>The item from the source that has the maximum value according to its key</returns>
        /// <exception cref="ArgumentNullException">If source or selector are null</exception>
        /// <exception cref="InvalidOperationException">If the source sequence contains no elements</exception>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) => 
            source.MaxBy(selector, Comparer<TKey>.Default);

        /// <summary>
        /// Returns the item from an enumerable that has the maximum value according
        /// to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of items in the source enumerable</typeparam>
        /// <typeparam name="TKey">The type of value being compared</typeparam>
        /// <param name="source">The source enumerable</param>
        /// <param name="selector">Determines the value from each item to use for comparison</param>
        /// <param name="comparer">The comparer to use on keys</param>
        /// <returns>The item from the source that has the maximum value according to its key</returns>
        /// <exception cref="ArgumentNullException">If source or selector are null</exception>
        /// <exception cref="InvalidOperationException">If the source sequence contains no elements</exception>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer) => 
            source.ExtremumBy(selector, comparer ?? Comparer<TKey>.Default);

        /// <summary>
        /// Returns the item from an enumerable that has the minimum value according
        /// to a key.  The default comparer is used.
        /// </summary>
        /// <typeparam name="TSource">The type of items in the source enumerable</typeparam>
        /// <typeparam name="TKey">The type of value being compared</typeparam>
        /// <param name="source">The source enumerable</param>
        /// <param name="selector">Determines the value from each item to use for comparison</param>
        /// <returns>The item from the source that has the minimum value according to its key</returns>
        /// <exception cref="ArgumentNullException">If source or selector are null</exception>
        /// <exception cref="InvalidOperationException">If the source sequence contains no elements</exception>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) => 
            source.MinBy(selector, Comparer<TKey>.Default);

        /// <summary>
        /// Returns the item from an enumerable that has the minimum value according
        /// to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of items in the source enumerable</typeparam>
        /// <typeparam name="TKey">The type of value being compared</typeparam>
        /// <param name="source">The source enumerable</param>
        /// <param name="selector">Determines the value from each item to use for comparison</param>
        /// <param name="comparer">The comparer to use on keys</param>
        /// <returns>The item from the source that has the minimum value according to its key</returns>
        /// <exception cref="ArgumentNullException">If source or selector are null</exception>
        /// <exception cref="InvalidOperationException">If the source sequence contains no elements</exception>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer) => 
            source.ExtremumBy(selector, new ReverseComparer<TKey>(comparer ?? Comparer<TKey>.Default));

        private static TSource ExtremumBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            TSource extremum = source.Aggregate((currentExtremum, element) =>
            {
                TKey extremumKey = selector(currentExtremum);
                TKey elementKey = selector(element);

                int result = comparer.Compare(elementKey, extremumKey);
                return result > 0 ? element : currentExtremum;
            });

            return extremum;
        }

        /// <summary>
        /// Lazily pipes the output of an enumerable to an action.
        /// </summary>
        /// <typeparam name="T">The type of items</typeparam>
        /// <param name="items">The items to pipe</param>
        /// <param name="action">The action to apply</param>
        /// <returns>The source enumerable</returns>
        public static IEnumerable<T> Tee<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
                yield return item;
            }
        }

        /// <summary>
        /// Iterates over an enumerable and adds each item to an existing collection.
        /// </summary>
        /// <remarks>
        /// This method may serve as an alternative to <see cref="Enumerable.ToList{TSource}"/> when
        /// an existing collection must be used instead of creating a new list.
        /// </remarks>
        /// <typeparam name="T">The type of items</typeparam>
        /// <param name="source">The items to iterate over</param>
        /// <param name="destination">An existing collection to add items to</param>
        public static void ToSink<T>(this IEnumerable<T> source, ICollection<T> destination)
        {
            foreach (var item in source)
                destination.Add(item);
        }

        /// <summary>
        /// Concatenates the items of a collection into a string.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="items">The items to join.</param>
        /// <param name="delimiter">The separator to use between items.</param>
        public static string ToDelimitedString<T>(this IEnumerable<T> items, string delimiter = ",") => String.Join(delimiter, items);

        /// <summary>
        /// Returns the first element of a sequence or <see cref="Option.None{T}"/> if the sequence is empty.
        /// </summary>
        /// <typeparam name="T">The type of items in the sequence</typeparam>
        /// <param name="source">The source items to query</param>
        /// <returns>An <see cref="Option.Some{T}"/> containing the first element of the sequence or <see cref="Option.None{T}"/></returns>
        public static Option<T> FirstOrNone<T>(this IEnumerable<T> source) => source.FirstOrNone(x => true);

        /// <summary>
        /// Returns the first element of a sequence that satisfies a condition or <see cref="Option.None{T}"/> if no such
        /// element is found.
        /// </summary>
        /// <typeparam name="T">The type of items in the sequence</typeparam>
        /// <param name="source">The source items to query</param>
        /// <param name="predicate">The condition an item must satisfy</param>
        /// <returns>An <see cref="Option.Some{T}"/> containing the first element of the sequence meeting the condition or <see cref="Option.None{T}"/></returns>
        public static Option<T> FirstOrNone<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            foreach (var item in source)
            {
                if (predicate(item))
                    return Option.Some(item);
            }

            return Option.None<T>();
        }

        /// <summary>
        /// Private class that provides the Slices enumerator.
        /// </summary>
        private class SliceEnumerable<T> : IEnumerable<IReadOnlyCollection<T>>
        {
            /// <summary>
            /// Creates a new Slice enumerable.
            /// </summary>
            public SliceEnumerable(IEnumerable<T> sourceItems, int sliceSize)
            {
                _sourceItems = sourceItems;
                _sliceSize = sliceSize;
            }

            #region IEnumerable Members

            /// <see cref="System.Collections.IEnumerable.GetEnumerator"/>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            #region IEnumerable<IEnumerable<T>> Members

            /// <see cref="System.Collections.Generic.IEnumerable{T}.GetEnumerator"/>
            public IEnumerator<IReadOnlyCollection<T>> GetEnumerator()
            {
                IList<T> buffer = new List<T>(_sliceSize);
                int itemCounter = 1;
                foreach (T item in _sourceItems)
                {
                    buffer.Add(item);
                    if (itemCounter % _sliceSize == 0)
                    {
                        yield return new List<T>(buffer);
                        buffer.Clear();
                    }

                    itemCounter++;
                }

                if (buffer.Count > 0)
                    yield return new List<T>(buffer);
            }

            #endregion

            private readonly IEnumerable<T> _sourceItems;
            private readonly int _sliceSize;
        }
    }
}