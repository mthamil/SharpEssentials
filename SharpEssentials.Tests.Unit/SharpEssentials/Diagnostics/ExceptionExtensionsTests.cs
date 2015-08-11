using System;
using System.Linq;
using SharpEssentials.Diagnostics;
using SharpEssentials.Testing;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Diagnostics
{
	public class ExceptionExtensionsTests
	{
		[Fact]
		public void Test_GetExceptionChain_SingleException()
		{
			// Arrange.
			var exception = new InvalidOperationException("message1");

			// Act.
			var chain = exception.GetExceptionChain();

			// Assert.
			var single = Assert.Single(chain);
			Assert.Equal(exception, single);
		}

		[Fact]
		public void Test_GetExceptionChain_MultipleExceptions()
		{
			// Arrange.
			var exception =
				new InvalidOperationException("message1",
					new ArgumentException("message2",
						new NotSupportedException("message3")));

			// Act.
			var chain = exception.GetExceptionChain();

			// Assert.
			AssertThat.SequenceEqual(new [] { "message1", "message2", "message3" }, chain.Select(e => e.Message));
			AssertThat.SequenceEqual(new [] { typeof(InvalidOperationException), typeof(ArgumentException), typeof(NotSupportedException) }, chain.Select(e => e.GetType()));
		} 
	}
}