using System;
using System.Collections.Generic;
using System.Linq;
using SharpEssentials.Reflection;
using SharpEssentials.Testing;
using Xunit;
using Xunit.Extensions;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Reflection
{
	public class ReflectionExtensionsTests
	{
		[Fact]
		public void Test_GetDefaultValue_ReferenceType()
		{
			// Act.
			object defaultValue = typeof(string).GetDefaultValue();

			// Assert.
			Assert.Equal(null, defaultValue);
		}

		[Fact]
		public void Test_GetDefaultValue_Integer()
		{
			// Act.
			object defaultValue = typeof(int).GetDefaultValue();

			// Assert.
			Assert.Equal(0, defaultValue);
		}

		[Fact]
		public void Test_GetDefaultValue_Boolean()
		{
			// Act.
			object defaultValue = typeof(bool).GetDefaultValue();

			// Assert.
			Assert.Equal(false, defaultValue);
		}

        [Theory]
        [InlineData(typeof(IList<string>), typeof(IList<>), true)]
        [InlineData(typeof(List<string>), typeof(List<>), true)]
        [InlineData(typeof(string), typeof(List<>), false)]
        public void Test_IsClosedTypeOf(Type closed, Type open, bool expected)
        {
            // Act.
            bool actual = closed.IsClosedTypeOf(open);

            // Assert.
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(typeof(string), typeof(IList<object>))]
        [InlineData(typeof(string), typeof(List<object>))]
        [InlineData(typeof(string), typeof(object))]
        public void Test_IsClosedTypeOf_With_NonOpen_Type(Type closed, Type open)
        {
            // Act/Assert.
            Assert.Throws<ArgumentException>(() =>
                closed.IsClosedTypeOf(open));
        }

        [Fact]
	    public void Test_GetDeclaredConstructor_Succeeds()
	    {
	        // Act.
            var constructor = typeof(string).GetDeclaredConstructor(typeof(char), typeof(int));

            // Assert.
            Assert.NotNull(constructor);
            Assert.Equal(typeof(string), constructor.DeclaringType);
            Assert.True(constructor.IsConstructor);
            AssertThat.SequenceEqual(constructor.GetParameters().Select(p => p.ParameterType), 
                                     new[] { typeof(char), typeof(int) });
	    }

        [Fact]
        public void Test_GetDeclaredConstructor_Fails()
        {
            // Act.
            var constructor = typeof(string).GetDeclaredConstructor(typeof(long), typeof(int));

            // Assert.
            Assert.Null(constructor);
        }
    }
}
