// Sharp Essentials
// Copyright 2015 Matthew Hamilton - matthamilton@live.com
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
using System.Threading.Tasks;
using System.Windows.Input;

namespace SharpEssentials.Controls.Mvvm.Commands.Builder
{
	/// <summary>
	/// Interface for a builder object that finishes constructing a command by specifying the
	/// actual operation that the command will execute.
	/// </summary>
	public interface ICommandCompleter
	{
		/// <summary>
		/// Sets the operation that a command will execute.
		/// </summary>
		/// <param name="operation">The parameterless operation to be executed</param>
		/// <returns>A new command</returns>
		ICommand Executes(Action operation);

		/// <summary>
		/// Sets the operation that a command will execute.
		/// </summary>
		/// <param name="operation">The operation to be executed</param>
		/// <returns>A new command</returns>
		ICommand Executes<T>(Action<T> operation);

        /// <summary>
        /// Sets the asynchronous operation that a command will execute.
        /// </summary>
        /// <param name="operation">The parameterless, asynchronous operation to be executed</param>
        /// <returns>A new command</returns>
        IAsyncCommand ExecutesAsync(Func<Task> operation);

        /// <summary>
        /// Sets the asynchronous operation that a command will execute.
        /// </summary>
        /// <param name="operation">The asynchronous operation to be executed</param>
        /// <returns>A new command</returns>
        IAsyncCommand ExecutesAsync<T>(Func<T, Task> operation);
    }
}