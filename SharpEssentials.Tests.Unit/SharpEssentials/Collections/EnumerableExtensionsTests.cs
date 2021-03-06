﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SharpEssentials.Collections;
using SharpEssentials.Testing;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Collections
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void Test_IEnumerator_ToEnumerable()
        {
            // Arrange.
            IList<double> values = new List<double> { 1.1, 2.2, 3.3, 4.4 };
            IEnumerator<double> enumerator = values.GetEnumerator();

            // Act.
            IEnumerable<double> valuesEnumerable = enumerator.ToEnumerable();

            // Assert.
            AssertThat.SequenceEqual(values.Select(item => item).ToList(), valuesEnumerable);
        }

        [Fact]
        public void Test_IEnumerator_ToEnumerable_NullSourceThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => ((IEnumerator<int>)null).ToEnumerable());
        }

        [Fact]
        public void Test_ValueType_ToEnumerable()
        {
            // Arrange.
            double value = 2.5;

            // Act.
            IEnumerable<double> enumerable = value.ToEnumerable();

            // Assert.
            Assert.Single(enumerable);
            Assert.Equal(value, enumerable.Single());
        }

        [Fact]
        public void Test_ReferenceType_ToEnumerable()
        {
            // Arrange.
            object o = new object();

            // Act.
            IEnumerable<object> enumerable = o.ToEnumerable();

            // Assert.
            Assert.Single(enumerable);
            Assert.Same(o, enumerable.Single());
        }

        [Theory]
        [InlineData(true,  new string[0])]						// The empty set should be a subset.
        [InlineData(true,  new[] { "3" })]						// Test a single element.
        [InlineData(true,  new[] { "1", "2" })]
        [InlineData(true,  new[] { "2", "1" })]					// Order should not matter.
        [InlineData(true,  new[] { "1", "2", "3", "4" })]		// This method does not determine Proper Subsets, so test the exactly contains case.
        [InlineData(false, new[] { "1", "2", "3", "4", "5" })]	// The subset contains more elements than the supposed superset.
        public void Test_Enumerable_IsSubsetOf(bool expected, IEnumerable<string> input)
        {
            // Arrange.
            var superset = new List<string> { "1", "2", "3", "4" };
            var subset = new List<string>(input);

            // Act.
            bool isSubset = subset.IsSubsetOf(superset);

            // Assert.
            Assert.Equal(expected, isSubset);
        }

        [Theory]
        [InlineData(true,  new string[0])]						// The empty set should be a subset.
        [InlineData(true,  new[] { "3" })]						// Test a single element.
        [InlineData(true,  new[] { "1", "2" })]
        [InlineData(true,  new[] { "2", "1" })]					// Order should not matter.
        [InlineData(true,  new[] { "1", "2", "3", "4" })]		// This method does not determine Proper Subsets, so test the exactly contains case.
        [InlineData(false, new[] { "1", "2", "3", "4", "5" })]	// The subset contains more elements than the supposed superset.
        public void Test_Enumerable_IsSubsetOf_HashSetOptimization(bool expected, IEnumerable<string> input)
        {
            // Arrange.
            IEnumerable<string> supersetHash = new HashSet<string> { "1", "2", "3", "4" };
            IEnumerable<string> subsetHash = new HashSet<string>(input);

            // Act.
            bool isSubset = subsetHash.IsSubsetOf(supersetHash);

            // Assert.
            Assert.Equal(expected, isSubset);
        }

        [Fact]
        public void Test_Slices()
        {
            // Arrange.
            IEnumerable<string> items = new List<string> { "1", "2", "3", "4", "5", "6", "7" };

            int sliceSize = 3;

            // Act.
            var slices = items.Slices(sliceSize);

            // Assert.
            Assert.Equal(3, slices.Count());

            using (var itemsEnumerator = items.GetEnumerator())
            using (var slicesEnumerator = slices.GetEnumerator())
            {
                int counter = 0;
                while (slicesEnumerator.MoveNext())
                {
                    var slice = slicesEnumerator.Current;
                    int sliceCount = slice.Count;

                    if (counter < 2)
                        Assert.Equal(sliceSize, sliceCount);
                    else
                        Assert.Equal(1, sliceCount);

                    using (var sliceEnumerator = slice.GetEnumerator())
                    {
                        while (sliceEnumerator.MoveNext() && itemsEnumerator.MoveNext())
                        {
                            Assert.Equal(itemsEnumerator.Current, sliceEnumerator.Current);
                        }
                    }

                    counter++;
                }
            }
        }

        [Fact]
        public void Test_Slices_NullSourceThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)null).Slices(1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-2)]
        public void Test_Slices_SliceSize_LessThan_One(int sliceSize)
        {
            // Arrange.
            IEnumerable<string> items = new List<string> { "1", "2", "3", "4" };

            // Act.
            var slices = items.Slices(sliceSize);

            // Assert.
            Assert.Empty(slices);
        }

        [Fact]
        public void Test_Slices_SliceSize_Equals_One()
        {
            // Arrange.
            IEnumerable<string> items = new List<string> { "1", "2", "3", "4", "5", "6", "7" };

            // Act.
            var slices = items.Slices(1);

            // Assert.
            Assert.Equal(items.Count(), slices.Count());

            using (var itemsEnumerator = items.GetEnumerator())
            using (var slicesEnumerator = slices.GetEnumerator())
            {
                while (itemsEnumerator.MoveNext() && slicesEnumerator.MoveNext())
                {
                    string item = itemsEnumerator.Current;
                    var slice = slicesEnumerator.Current;

                    Assert.Equal(1, slice.Count);
                    Assert.Equal(item, slice.Single());
                }
            }
        }

        [Fact]
        public void Test_Slices_SliceSize_Equals_Count()
        {
            // Arrange.
            IEnumerable<string> items = new List<string> { "1", "2", "3", "4", "5", "6", "7" };

            // Act.
            var slices = items.Slices(items.Count());

            // Assert.
            Assert.Single(slices);
            AssertThat.SequenceEqual(items, slices.Single());
        }

        [Fact]
        public void Test_MaxBy_NullSourceThrowsException()
        {
            // Arrange.
            IEnumerable<TestContainer<int>> items = null;

            // Act/Assert.
            Assert.Throws<ArgumentNullException>(() => items.MaxBy(item => item.Item));
        }

        [Fact]
        public void Test_MaxBy_EmptySourceThrowsException()
        {
            // Arrange.
            IEnumerable<TestContainer<int>> items = new List<TestContainer<int>>();

            // Act/Assert.
            Assert.Throws<InvalidOperationException>(() => items.MaxBy(item => item.Item));
        }

        [Fact]
        public void Test_MaxBy_NullSelectorThrowsException()
        {
            // Arrange.
            IEnumerable<TestContainer<int>> items = new List<TestContainer<int>>();
            Func<TestContainer<int>, int> selector = null;

            // Act/Assert.
            Assert.Throws<ArgumentNullException>(() => items.MaxBy(selector));
        }

        [Fact]
        public void Test_MaxBy_DefaultComparer()
        {
            // Arrange.
            IEnumerable<TestContainer<int>> items = Enumerable.Range(1, 10).Select(i => new TestContainer<int> { Item = i });

            // Act.
            TestContainer<int> max = items.MaxBy(item => item.Item);

            // Assert.
            Assert.Equal(10, max.Item);
        }

        [Fact]
        public void Test_MaxBy_NullComparerUsesDefault()
        {
            // Arrange.
            IEnumerable<TestContainer<int>> items = Enumerable.Range(1, 10).Select(i => new TestContainer<int> { Item = i });

            // Act.
            TestContainer<int> max = items.MaxBy(item => item.Item, null);

            // Assert.
            Assert.Equal(10, max.Item);
        }

        [Fact]
        public void Test_MaxBy_CustomComparer()
        {
            // Arrange.
            IEnumerable<TestContainer<string>> items = new List<TestContainer<string>>
            {
                new TestContainer<string> { Item = "a" },
                new TestContainer<string> { Item = "b" },
                new TestContainer<string> { Item = "c" }
            };

            // Act.
            TestContainer<string> max = items.MaxBy(item => item.Item, StringComparer.OrdinalIgnoreCase);

            // Assert.
            Assert.Equal("c", max.Item);
        }

        [Fact]
        public void Test_MaxBy_SingleItem()
        {
            // Arrange.
            IEnumerable<TestContainer<int>> items = new List<TestContainer<int>> 
            { 
                new TestContainer<int> { Item = 3 } 
            };

            // Act.
            TestContainer<int> max = items.MaxBy(item => item.Item);

            // Assert.
            Assert.Equal(3, max.Item);
        }

        [Fact]
        public void Test_MaxBy_EqualItems()
        {
            // Arrange.
            IEnumerable<TestContainer<int>> items = new List<TestContainer<int>> 
            { 
                new TestContainer<int> { Item = 3 },
                new TestContainer<int> { Item = 3 },
                new TestContainer<int> { Item = 3 }
            };

            // Act.
            TestContainer<int> max = items.MaxBy(item => item.Item);

            // Assert.
            Assert.Equal(3, max.Item);
            Assert.Same(items.First(), max);
        }

        [Fact]
        public void Test_MinBy_NullSourceThrowsException()
        {
            // Arrange.
            IEnumerable<TestContainer<int>> items = null;

            // Act/Assert.
            Assert.Throws<ArgumentNullException>(() => items.MinBy(item => item.Item));
        }

        [Fact]
        public void Test_MinBy_EmptySourceThrowsException()
        {
            // Arrange.
            IEnumerable<TestContainer<int>> items = new List<TestContainer<int>>();

            // Act/Assert.
            Assert.Throws<InvalidOperationException>(() => items.MinBy(item => item.Item));
        }

        [Fact]
        public void Test_MinBy_NullSelectorThrowsException()
        {
            // Arrange.
            IEnumerable<TestContainer<int>> items = new List<TestContainer<int>>();
            Func<TestContainer<int>, int> selector = null;

            // Act/Assert.
            Assert.Throws<ArgumentNullException>(() => items.MinBy(selector));
        }

        [Fact]
        public void Test_MinBy_DefaultComparer()
        {
            // Arrange.
            IEnumerable<TestContainer<int>> items = Enumerable.Range(1, 10).Select(i => new TestContainer<int> { Item = i });

            // Act.
            TestContainer<int> min = items.MinBy(item => item.Item);

            // Assert.
            Assert.Equal(1, min.Item);
        }

        [Fact]
        public void Test_MinBy_NullComparerUsesDefault()
        {
            // Arrange.
            IEnumerable<TestContainer<int>> items = Enumerable.Range(1, 10).Select(i => new TestContainer<int> { Item = i });

            // Act.
            TestContainer<int> min = items.MinBy(item => item.Item, null);

            // Assert.
            Assert.Equal(1, min.Item);
        }

        [Fact]
        public void Test_MinBy_CustomComparer()
        {
            // Arrange.
            IEnumerable<TestContainer<string>> items = new List<TestContainer<string>>
            {
                new TestContainer<string> { Item = "a" },
                new TestContainer<string> { Item = "b" },
                new TestContainer<string> { Item = "c" }
            };

            // Act.
            TestContainer<string> min = items.MinBy(item => item.Item, StringComparer.OrdinalIgnoreCase);

            // Assert.
            Assert.Equal("a", min.Item);
        }

        [Fact]
        public void Test_MinBy_SingleItem()
        {
            // Arrange.
            IEnumerable<TestContainer<int>> items = new List<TestContainer<int>> 
            { 
                new TestContainer<int> { Item = 3 } 
            };

            // Act.
            TestContainer<int> min = items.MinBy(item => item.Item);

            // Assert.
            Assert.Equal(3, min.Item);
        }

        [Fact]
        public void Test_MinBy_EqualItems()
        {
            // Arrange.
            IEnumerable<TestContainer<int>> items = new List<TestContainer<int>> 
            { 
                new TestContainer<int> { Item = 3 },
                new TestContainer<int> { Item = 3 },
                new TestContainer<int> { Item = 3 }
            };

            // Act.
            TestContainer<int> min = items.MinBy(item => item.Item);

            // Assert.
            Assert.Equal(3, min.Item);
            Assert.Same(items.First(), min);
        }

        private class TestContainer<T>
        {
            public T Item { get; set; }
        }

        [Fact]
        public void Test_Tee()
        {
            // Arrange.
            var input = new[] { 1, 2, 3 };
            var output = new List<int>(input.Length);

            // Act.
            var items = input.Tee(output.Add).Select(x => x.ToString(CultureInfo.InvariantCulture));

            // Assert.
            Assert.Empty(output);	// Tests laziness.

            // Act.
            items = items.ToList();

            // Assert.
            AssertThat.SequenceEqual(input, output);
            AssertThat.SequenceEqual(new [] { "1", "2", "3" }, items);
        }

        [Fact]
        public void Test_ToSink()
        {
            // Arrange.
            var input = new[] { 4, 5, 6 };
            var sink = new List<int> { 1, 2, 3 };

            // Act.
            input.ToSink(sink);

            // Assert.
            AssertThat.SequenceEqual(new [] { 1, 2, 3, 4, 5, 6 }, sink);
        }

        public static IEnumerable<object[]> FirstOrNoneWithPredicateData => 
            new TheoryData<Option<string>, IEnumerable<string>>
            {
                { "2",					 new [] { "1", "2", "3", "4" } },
                { "4",					 new [] { "4", "3", "2", "1" } },
                { Option.None<string>(), new [] { "1", "3", "5", "7" } },
                { Option.None<string>(), Enumerable.Empty<string>() },
            };

        [Theory]
        [MemberData(nameof(FirstOrNoneWithPredicateData))]
        public void Test_FirstOrNone_WithPredicate(Option<string> expected, IEnumerable<string> input)
        {
            // Act.
            var actual = input.FirstOrNone(x => Int32.Parse(x) % 2 == 0);

            // Assert.
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_FirstOrNone_WithPredicate_NullSource_ThrowsException()
        {
            // Act/Assert.
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)null).FirstOrNone(x => x == 2));
        }

        [Fact]
        public void Test_FirstOrNone_WithPredicate_NullPredicate_ThrowsException()
        {
            // Act/Assert.
            Assert.Throws<ArgumentNullException>(() => new[] { 1, 2, 3 }.FirstOrNone(null));
        }

        public static IEnumerable<object[]> FirstOrNoneData => 
            new TheoryData<Option<string>, IEnumerable<string>>
            {
                { "1",					 new [] { "1", "2", "3", "4" } },
                { "4",					 new [] { "4" } },
                { Option.None<string>(), Enumerable.Empty<string>() },
            };

        [Theory]
        [MemberData(nameof(FirstOrNoneData))]
        public void Test_FirstOrNone(Option<string> expected, IEnumerable<string> input)
        {
            // Act.
            var actual = input.FirstOrNone();

            // Assert.
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_FirstOrNone_NullSource_ThrowsException()
        {
            // Act/Assert.
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)null).FirstOrNone());
        }

        [Theory]
        [InlineData(new object[] {1, 2, 3, 4}, ",", "1,2,3,4")]
        [InlineData(new[] { "a", "b", "c", "d" }, ":", "a:b:c:d")]
        [InlineData(new object[0], ",", "")]
        public void Test_ToDelimitedString<T>(IEnumerable<T> items, string delimiter, string expected)
        {
            // Act.
            var actual = items.ToDelimitedString(delimiter);

            // Assert.
            Assert.Equal(expected, actual);
        }
    }
}