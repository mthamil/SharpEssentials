using System;
using SharpEssentials.Weak;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Weak
{
    public class WeakReferenceExtensionsTests
    {
        [Fact]
        public void Test_TryGetTarget_When_Target_Not_Null()
        {
            // Arrange.
            var target = new object();
            var reference = new WeakReference<object>(target);

            // Act.
            var actual = reference.TryGetTarget();

            // Assert.
            Assert.True(actual.HasValue);
        }

        [Fact]
        public void Test_TryGetTarget_When_Target_Null()
        {
            // Arrange.
            var reference = new Func<WeakReference<object>>(() =>
            {
                var target = new object();
                return new WeakReference<object>(target);
            })();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Act.
            var actual = reference.TryGetTarget();

            // Assert.
            Assert.False(actual.HasValue);
        }
    }
}