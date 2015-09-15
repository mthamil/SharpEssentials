using System;
using System.Threading;
using System.Threading.Tasks;
using SharpEssentials.Concurrency;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Concurrency
{
    public class WaitHandleExtensionsTests : IDisposable
    {
        public WaitHandleExtensionsTests()
        {
            _handle = new ManualResetEvent(false);
        }

        [Fact]
        public async Task Test_AsTask()
        {
            // Arrange.
            var setTask = Task.Run(async () =>
            {
                await Task.Delay(200);
                _handle.Set();
            });

            // Act.
            await _handle.AsTask();

            // Assert.
            Assert.True(setTask.IsCompleted);
        }

        [Fact]
        public async Task Test_AsTask_With_Timeout()
        {
            // Act/Assert.
            await Assert.ThrowsAsync<TaskCanceledException>(() => 
                _handle.AsTask(TimeSpan.FromMilliseconds(100)));
        }

        public void Dispose()
        {
            _handle.Dispose();
        }

        private readonly ManualResetEvent _handle;
    }
}