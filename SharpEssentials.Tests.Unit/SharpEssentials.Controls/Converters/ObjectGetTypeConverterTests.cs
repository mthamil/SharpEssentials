using System;
using System.Collections.Generic;
using SharpEssentials.Controls.Converters;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Controls.Converters
{
    public class ObjectGetTypeConverterTests
    {
        public static IEnumerable<object[]> ConvertData =>
            new TheoryData<object, Type>
            {
                { "test",           typeof(string) },
                { new object(),     typeof(object) },
                { new List<int>(),  typeof(List<int>) },
                { null,             typeof(object) }
            };

        [Theory]
        [MemberData(nameof(ConvertData))]
        public void Test_Convert(object input, Type expected)
        {
            // Act.
            var actual = _underTest.Convert(input, null, null, null);

            // Assert.
            Assert.Equal(expected, actual);
        }

        private readonly ObjectGetTypeConverter _underTest = new ObjectGetTypeConverter();
    }
}