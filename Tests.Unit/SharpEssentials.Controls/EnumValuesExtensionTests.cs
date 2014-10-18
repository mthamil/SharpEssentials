using System;
using Moq;
using SharpEssentials.Controls.Markup;
using SharpEssentials.Testing;
using Xunit;

namespace Tests.Unit.SharpEssentials.Controls
{
	public class EnumValuesExtensionTests
	{
		[Fact]
		public void Test_ProvideValue()
		{
			// Arrange.
			var extension = new EnumValuesExtension(typeof(TestEnum));

			// Act.
			var values = (TestEnum[])extension.ProvideValue(new Mock<IServiceProvider>().Object);

			// Assert.
			AssertThat.SequenceEqual(new[] { TestEnum.Value1, TestEnum.Value2, TestEnum.Value3 }, values);
		} 

		private enum TestEnum
		{
			Value1,
			Value2,
			Value3
		}
	}
}