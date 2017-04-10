// Sharp Essentials
// Copyright 2017 Matthew Hamilton - matthamilton@live.com
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SharpEssentials.Collections
{
    /// <summary>
    /// Contains extension methods pertaining to the combination of tasks and enumerables.
    /// </summary>
    public static class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Allows await-ing on an enumerable of Tasks.
        /// </summary>
        /// <param name="tasks">The tasks to await</param>
        /// <returns>An awaiter for the completion of the tasks</returns>
        public static TaskAwaiter GetAwaiter(this IEnumerable<Task> tasks) => 
            Task.WhenAll(tasks).GetAwaiter();

        /// <summary>
        /// Allows await-ing on an enumerable of Task&lt;T&gt;s.
        /// </summary>
        /// <param name="tasks">The tasks to await</param>
        /// <returns>An awaiter for the completion of the tasks</returns>
        public static TaskAwaiter<T[]> GetAwaiter<T>(this IEnumerable<Task<T>> tasks) => 
            Task.WhenAll(tasks).GetAwaiter();

        /// <summary>
        /// Concatenates two asynchronous sequences.
        /// </summary>
        /// <remarks>Convenient when used for aggregation with tasks.</remarks>
        /// <typeparam name="T">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">The first asynchronous sequence to concatenate.</param>
        /// <param name="second">The asynchronous sequence to concatenate to the first sequence.</param>
        /// <returns>A task representing the concatenated elements of the two input sequences.</returns>
        public static async Task<IEnumerable<T>> Concat<T>(this Task<IEnumerable<T>> first, Task<IEnumerable<T>> second) => 
            (await first.ConfigureAwait(false)).Concat(
             await second.ConfigureAwait(false));
    }
}