using System.Globalization;
using System.Windows.Data;
using Moq;
using SharpEssentials.Controls.Converters;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Controls.Converters
{
	public class ReverseConverterTests
	{
		[Fact]
		public void Test_Convert()
		{
			// Arrange.
			var inner = new Mock<IValueConverter>();
			converter.InnerConverter = inner.Object;

			// Act.
			converter.Convert("test", typeof(int), "parameter", CultureInfo.InvariantCulture);

			// Assert.
			inner.Verify(c => c.ConvertBack("test", typeof(int), "parameter", CultureInfo.InvariantCulture));
		}

		[Fact]
		public void Test_ConvertBack()
		{
			// Arrange.
			var inner = new Mock<IValueConverter>();
			converter.InnerConverter = inner.Object;

			// Act.
			converter.ConvertBack("test", typeof(int), "parameter", CultureInfo.InvariantCulture);

			// Assert.
			inner.Verify(c => c.Convert("test", typeof(int), "parameter", CultureInfo.InvariantCulture));
		}

		private readonly ReverseConverter converter = new ReverseConverter();
	}
}