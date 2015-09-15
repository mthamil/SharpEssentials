// Sharp Essentials
// Copyright 2015 Matthew Hamilton - matthamilton@live.com
// Copyright 2013 Stephen Cleary
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

using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpEssentials.Concurrency
{
    /// <summary>
    /// Contains utility methods that help in using <see cref="WaitHandle"/>s with
    /// <see cref="Task"/>-based code.
    /// </summary>
    public static class WaitHandleExtensions
    {
        /// <summary>
        /// Returns a <see cref="Task"/> that will complete when the given <see cref="WaitHandle"/> completes.
        /// </summary>
        /// <param name="handle">The handle to wait on.</param>
        /// <remarks>See http://stackoverflow.com/questions/18756354/wrapping-manualresetevent-as-awaitable-task</remarks>
        public static Task AsTask(this WaitHandle handle)
        {
            return AsTask(handle, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Returns a <see cref="Task"/> that will complete when the given <see cref="WaitHandle"/> completes
        /// or the <paramref name="timeout"/> expires.
        /// </summary>
        /// <param name="handle">The handle to wait on.</param>
        /// <param name="timeout">The amount of time to wait for the handle to complete.</param>
        /// <remarks>See http://stackoverflow.com/questions/18756354/wrapping-manualresetevent-as-awaitable-task</remarks>
        public static Task AsTask(this WaitHandle handle, TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<object>();
            var registration = ThreadPool.RegisterWaitForSingleObject(handle, (state, timedOut) =>
            {
                var localTcs = (TaskCompletionSource<object>)state;
                if (timedOut)
                    localTcs.TrySetCanceled();
                else
                    localTcs.TrySetResult(null);
            }, tcs, timeout, executeOnlyOnce: true);

            tcs.Task.ContinueWith((_, state) =>
                ((RegisteredWaitHandle)state).Unregister(null), 
                    registration, TaskScheduler.Default);

            return tcs.Task;
        }
    }
}