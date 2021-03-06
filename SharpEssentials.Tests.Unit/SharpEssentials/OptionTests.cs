﻿using System;
using System.Globalization;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials
{
    public class OptionTests
    {
        [Fact]
        public void Test_Some()
        {
            // Arrange.
            var some = Option.Some("some");

            // Act.
            bool hasValue = some.HasValue;
            var value = some.Value;

            // Assert.
            Assert.True(hasValue);
            Assert.Equal("some", value);
        }

        [Fact]
        public void Test_None()
        {
            // Arrange.
            var stringNone = Option.None<string>();

            // Act/Assert.
            Assert.False(stringNone.HasValue);
            Assert.Throws<InvalidOperationException>(() => { var test = stringNone.Value; });
        }

        [Fact]
        public void Test_None_Equality()
        {
            var stringNone = Option.None<string>();
            var intNone = Option.None<int>();

            Assert.NotSame(stringNone, intNone);
            Assert.False(stringNone.Equals(intNone));

            Assert.Same(Option.None<string>(), Option.None<string>());
            Assert.Equal(Option.None<string>(), Option.None<string>());
        }

        [Fact]
        public void Test_Some_Equality()
        {
            var some = Option.Some("some");
            Assert.Equal(some, some);

            // Test that equality is determined by Some's value.
            var some2 = Option.Some("some");
            Assert.NotSame(some, some2);
            Assert.Equal(some, some2);

            some2 = Option.Some("some2");
            Assert.NotEqual(some, some2);

            var intSome = Option.Some(3);
            Assert.Equal(3, intSome.Value);
            Assert.False(intSome.Equals(some2));
        }

        [Fact]
        public void Test_Some_CannotBeNull()
        {
            // Act/Assert.
            Assert.Throws<ArgumentNullException>(() => Option.Some<string>(null));
        }

        [Fact]
        public void Test_Some_Conversion_StringLiteral()
        {
            // Act.
            Option<string> some = "test";

            // Assert.
            Assert.True(some.HasValue);
            Assert.Equal("test", some.Value);
        }

        [Fact]
        public void Test_Some_Conversion_ValueType()
        {
            // Act.
            Option<int> some = 1;

            // Assert.
            Assert.True(some.HasValue);
            Assert.Equal(1, some.Value);
        }

        [Fact]
        public void Test_Some_Conversion_ReferenceType()
        {
            // Arrange.
            var value = new object();

            // Act.
            Option<object> some = value;

            // Assert.
            Assert.True(some.HasValue);
            Assert.Equal(value, some.Value);
        }

        [Fact]
        public void Test_None_Conversion_ReferenceType()
        {
            // Arrange.
            string value = null;

            // Act.
            Option<string> none = value;

            // Assert.
            Assert.NotNull(none);
            Assert.False(none.HasValue);
        }

        [Theory]
        [InlineData(true, "value")]
        [InlineData(false, null)]
        public void Test_Option_From(bool expectValue, string value)
        {
            // Act.
            Option<string> option = Option.From(value);

            // Assert.
            Assert.Equal(expectValue, option.HasValue);
        }

        [Fact]
        public void Test_Some_Select()
        {
            // Arrange.
            Option<int> someInt = 1;

            // Act.
            Option<string> result = someInt.Select(x => x.ToString(CultureInfo.InvariantCulture));
            Option<string> linqResult = from x in someInt select x.ToString(CultureInfo.InvariantCulture);

            // Assert.
            Assert.True(result.HasValue);
            Assert.Equal("1", result.Value);

            Assert.True(linqResult.HasValue);
            Assert.Equal("1", linqResult.Value);
        }

        [Fact]
        public void Test_Some_SelectMany()
        {
            // Arrange.
            Option<int> someInt = 1;

            // Act.
            Option<string> result = someInt.SelectMany(x => new ReturnsOption(x).Get(true));
            Option<string> linqResult = 
                from x in someInt
                from y in new ReturnsOption(x).Get(true)
                select y;

            // Assert.
            Assert.True(result.HasValue);
            Assert.Equal("1", result.Value);

            Assert.True(linqResult.HasValue);
            Assert.Equal("1", linqResult.Value);
        }

        [Fact]
        public void Test_Some_SelectMany_NoResult()
        {
            // Arrange.
            Option<int> someInt = 1;

            // Act.
            Option<string> result = someInt.SelectMany(x => new ReturnsOption(x).Get(false));
            Option<string> linqResult =
                from x in someInt
                from y in new ReturnsOption(x).Get(false)
                select y;

            // Assert.
            Assert.False(result.HasValue);
            Assert.False(linqResult.HasValue);
        }

        [Fact]
        public void Test_Some_Where_ConditionMet()
        {
            // Arrange.
            Option<int> someInt = 6;

            // Act.
            var result = someInt.Where(x => x % 2 == 0);
            var linqResult =
                from x in someInt
                where x % 2 == 0
                select x;

            // Assert.
            Assert.True(result.HasValue);
            Assert.Equal(someInt, result);

            Assert.True(linqResult.HasValue);
            Assert.Equal(someInt, linqResult);
        }

        [Fact]
        public void Test_Some_Where_ConditionNotMet()
        {
            // Arrange.
            Option<int> someInt = 5;

            // Act.
            var result = someInt.Where(x => x % 2 == 0);
            var linqResult =
                from x in someInt
                where x % 2 == 0
                select x;

            // Assert.
            Assert.False(result.HasValue);
            Assert.False(linqResult.HasValue);
        }

        [Fact]
        public void Test_Some_Apply()
        {
            // Arrange.
            Option<int> someInt = 5;
            int sum = 2;

            // Act.
            var result = someInt.Apply(x => sum += x);

            // Assert.
            Assert.Equal(7, sum);
            Assert.Equal(someInt, result);
        }

        [Fact]
        public void Test_Some_OrElse()
        {
            // Arrange.
            Option<int> someInt = 5;
            bool invoked = false;

            // Act.
            var result = someInt.OrElse(() =>
            { 
                invoked = true;
                return Option.None<int>();
            });

            // Assert.
            Assert.False(invoked);
            Assert.Equal(someInt.Value, result.Value);
        }

        [Fact]
        public void Test_Some_OrElse_No_Alternative()
        {
            // Arrange.
            Option<int> someInt = 5;
            bool invoked = false;

            // Act.
            var result = someInt.OrElse(() => invoked = true);

            // Assert.
            Assert.False(invoked);
            Assert.Equal(someInt.Value, result.Value);
        }

        [Fact]
        public void Test_Some_GetOrElse()
        {
            // Arrange.
            Option<int> someInt = 5;
            bool invoked = false;

            // Act.
            var result = someInt.GetOrElse(() =>
            {
                invoked = true;
                return 0;
            });

            // Assert.
            Assert.False(invoked);
            Assert.Equal(5, result);
        }

        [Fact]
        public void Test_None_Select()
        {
            // Arrange.
            var noneInt = Option.None<int>();

            // Act.
            Option<string> result = noneInt.Select(x => x.ToString(CultureInfo.InvariantCulture));
            Option<string> linqResult = from x in noneInt select x.ToString(CultureInfo.InvariantCulture);

            // Assert.
            Assert.False(result.HasValue);
            Assert.False(linqResult.HasValue);
        }

        [Fact]
        public void Test_None_SelectMany()
        {
            // Arrange.
            var noneInt = Option.None<int>();

            // Act.
            Option<string> result = noneInt.SelectMany(x => new ReturnsOption(x).Get(true));
            Option<string> linqResult =
                from x in noneInt
                from y in new ReturnsOption(x).Get(true)
                select y;

            // Assert.
            Assert.False(result.HasValue);
            Assert.False(linqResult.HasValue);
        }

        [Fact]
        public void Test_None_Where()
        {
            // Arrange.
            var noneInt = Option.None<int>();

            // Act.
            var result = noneInt.Where(x => x % 2 == 0);
            var linqResult =
                from x in noneInt
                where x % 2 == 0
                select x;

            // Assert.
            Assert.False(result.HasValue);
            Assert.False(linqResult.HasValue);
        }

        [Fact]
        public void Test_None_Apply()
        {
            // Arrange.
            var noneInt = Option.None<int>();
            int sum = 2;

            // Act.
            var result = noneInt.Apply(x => sum += x);

            // Assert.
            Assert.Equal(2, sum);
            Assert.Equal(noneInt, result);
        }

        [Fact]
        public void Test_None_OrElse()
        {
            // Arrange.
            var noneInt = Option.None<int>();
            bool invoked = false;

            // Act.
            var result = noneInt.OrElse(() =>
            {
                invoked = true;
                return Option.Some(5);
            });

            // Assert.
            Assert.True(invoked);
            Assert.Equal(5, result.Value);
        }

        [Fact] public void Test_None_OrElse_No_Alternative()
        {
            // Arrange.
            var noneInt = Option.None<int>();
            bool invoked = false;

            // Act.
            var result = noneInt.OrElse(() => invoked = true);

            // Assert.
            Assert.True(invoked);
            Assert.Equal(noneInt, result);
        }

        [Fact]
        public void Test_None_GetOrElse()
        {
            // Arrange.
            var noneInt = Option.None<int>();
            bool invoked = false;

            // Act.
            var result = noneInt.GetOrElse(() =>
            {
                invoked = true;
                return 5;
            });

            // Assert.
            Assert.True(invoked);
            Assert.Equal(5, result);
        }

        private class ReturnsOption
        {
            private readonly int _val;

            public ReturnsOption(int val)
            {
                _val = val;
            }

            public Option<string> Get(bool shouldHaveValue)
            {
                if (shouldHaveValue)
                    return Option.Some(_val.ToString(CultureInfo.InvariantCulture));

                return Option.None<string>();
            }
        }
    }
}