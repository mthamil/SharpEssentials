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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpEssentials.Collections;

namespace SharpEssentials.Concurrency
{
    /// <summary>
    /// Contains Task utility methods.
    /// </summary>
    public static class Tasks
    {
        /// <summary>
        /// Creates an already canceled task.
        /// </summary>
        /// <typeparam name="TResult">The type of the result the task was supposed to return</typeparam>
        /// <returns>A canceled task</returns>
        public static Task<TResult> FromCanceled<TResult>()
        {
            var taskSource = new TaskCompletionSource<TResult>();
            taskSource.SetCanceled();
            return taskSource.Task;
        }

        /// <summary>
        /// Creates an already canceled task.
        /// </summary>
        /// <returns>A canceled task</returns>
        public static Task FromCanceled()
        {
            return FromCanceled<AsyncUnit>();
        }

        /// <summary>
        /// Creates an already completed task from an exception.
        /// </summary>
        /// <typeparam name="TException">The type of exception that was "thrown"</typeparam>
        /// <returns>A Task that has failed due to the given exception</returns>
        public static Task FromException<TException>()
            where TException : Exception, new()
        {
            return Task.FromException(new TException());
        }

        /// <summary>
        /// Creates an already completed task from a collection of exceptions.
        /// </summary>
        /// <typeparam name="TResult">The expected result type</typeparam>
        /// <param name="first">The first exception</param>
        /// <param name="exceptions">Existing exceptions</param>
        /// <returns>A Task that has failed due to the given exceptions</returns>
        public static Task<TResult> FromExceptions<TResult>(Exception first, params Exception[] exceptions)
        {
            var taskSource = new TaskCompletionSource<TResult>();
            taskSource.SetException(first.ToEnumerable().Concat(exceptions));
            return taskSource.Task;
        }

        /// <summary>
        /// Creates an already completed task from a collection of exceptions.
        /// </summary>
        /// <param name="first">The first exception</param>
        /// <param name="exceptions">Existing exceptions</param>
        /// <returns>A Task that has failed due to the given exceptions</returns>
        public static Task FromExceptions(Exception first, params Exception[] exceptions)
        {
            return FromExceptions<AsyncUnit>(first, exceptions);
        }

        /// <summary>
        /// Returns an already completed task with an empty enumerable result.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <returns>A completed task whose result is an empty enumerable.</returns>
        public static Task<IEnumerable<T>> Empty<T>()
        {
            return Task.FromResult(Enumerable.Empty<T>());
        }

        /// <summary>
        /// Instead of using System.Object, this type is a clearer indication that a Task does not return a result.
        /// </summary>
        struct AsyncUnit { }
    }
}