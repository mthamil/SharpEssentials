using System.Collections.Generic;
using SharpEssentials.Collections;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Collections
{
	public class DictionaryExtensionsTests
	{
		[Fact]
		public void Test_TryGetValue_Key_Exists()
		{
			// Arrange.
			var dict = new Dictionary<int, string> { [5] = "value" };

			// Act.
			var result = dict.TryGetValue(5);

			// Assert.
			Assert.True(result.HasValue);
			Assert.Equal("value", result.Value);
		}

		[Fact]
		public void Test_TryGetValue_Key_DoesNotExist()
		{
			// Arrange.
			var dict = new Dictionary<int, string> { [5] = "value" };

			// Act.
			var result = dict.TryGetValue(2);

			// Assert.
			Assert.False(result.HasValue);
		}
	}
}