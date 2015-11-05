using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpEssentials.Collections;
using SharpEssentials.Testing;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Collections
{
    public class AsyncEnumerableExtensionsTests
    {
        [Fact]
        public async Task Test_GetAwaiter_EnumerableOfTasks_WithResults()
        {
            // Act.
            var results = await Enumerable.Range(0, 5).Select(Task.FromResult);

            // Assert.
            AssertThat.SequenceEqual(new[] {0, 1, 2, 3, 4}, results);
        }

        [Fact]
        public async Task Test_GetAwaiter_EnumerableOfTasks()
        {
            // Arrange.
            var results = new bool[5];

            // Act.
            await Enumerable.Range(0, 5).Select<int, Task>(async i =>
            {
                await Task.CompletedTask;
                results[i] = true;
            });

            // Assert.
            foreach (var result in results)
                Assert.True(result);
        }

        [Fact]
        public async Task Test_Concat_Task_Enumerables()
        {
            // Arrange.
            var first = Task.FromResult<IEnumerable<int>>(new[] {1, 2, 3});
            var second = Task.FromResult<IEnumerable<int>>(new List<int> {4, 5, 6});

            // Act.
            var result = await first.Concat(second);

            // Assert.
            AssertThat.SequenceEqual(new[] {1, 2, 3, 4, 5, 6}, result);
        }
    }
}
