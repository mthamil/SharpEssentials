using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials
{
    public class DisposableAdapterTests
    {
        [Fact]
        public void Test_Disposal()
        {
            // Arrange.
            string disposed = null;

            var underTest = new DisposableAdapter<string>("test", x => disposed = x);

            // Act.
            underTest.Dispose();

            // Assert.
            Assert.True(underTest.IsDisposed);
            Assert.Equal("test", disposed);
        } 
    }
}