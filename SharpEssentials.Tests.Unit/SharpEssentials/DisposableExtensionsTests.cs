using System;
using Moq;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials
{
	public class DisposableExtensionsTests
	{
        [Fact]
        public void Test_Lazy_Dispose_Does_Not_Call_Dispose_When_Value_Not_Created()
        {
            // Arrange.
            var disposable = new Mock<IDisposable>();
            var lazy = new Lazy<IDisposable>(() => disposable.Object);

            // Act.
            lazy.Dispose();

            // Assert.
            disposable.Verify(d => d.Dispose(), Times.Never);
        }

        [Fact]
        public void Test_Lazy_Dispose_Calls_Dispose_When_Value_Created()
        {
            // Arrange.
            var disposable = new Mock<IDisposable>();
            var lazy = new Lazy<IDisposable>(() => disposable.Object);

            // Act.
            var value = lazy.Value; // Trigger value intialization.
            lazy.Dispose();

            // Assert.
            disposable.Verify(d => d.Dispose(), Times.Once);
        }
	}
}