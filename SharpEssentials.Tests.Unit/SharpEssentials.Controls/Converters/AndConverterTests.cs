using System.Globalization;
using SharpEssentials.Controls.Converters;
using Xunit;
using Xunit.Extensions;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Controls.Converters
{
	public class AndConverterTests
	{
		[Theory]
		[InlineData(true,  new[] { true,  true,  true })]
		[InlineData(false, new[] { true,  false, false })]
		[InlineData(false, new[] { false, true,  false })]
		[InlineData(false, new[] { false, false, true })]
		[InlineData(false, new[] { true,  false, true })]
		[InlineData(false, new[] { false, false, false })]
		public void Test_Convert(bool expected, bool[] input)
		{
			// Arrange.
			object[] values = new object[input.Length];
			input.CopyTo(values, 0);

			// Act.
			bool actual = (bool)converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

			// Assert.
			Assert.Equal(expected, actual);
		}

		private readonly AndConverter converter = new AndConverter();
	}
}