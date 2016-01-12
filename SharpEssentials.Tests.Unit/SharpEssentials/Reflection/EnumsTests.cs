using SharpEssentials.Reflection;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Reflection
{
    public class EnumsTests
    {
        [Fact]
        public void Test_TryParse_Succeeds()
        {
            // Act.
            var actual = Enums.TryParse<TestEnum>("Value1");

            // Assert.
            Assert.True(actual.HasValue);
            Assert.Equal(TestEnum.Value1, actual.Value);
        }

        [Fact]
        public void Test_TryParse_Fails()
        {
            // Act.
            var actual = Enums.TryParse<TestEnum>("Value4");

            // Assert.
            Assert.False(actual.HasValue);
        }

        enum TestEnum
        {
            Value1,
            Value2,
            Value3
        }
    }
}