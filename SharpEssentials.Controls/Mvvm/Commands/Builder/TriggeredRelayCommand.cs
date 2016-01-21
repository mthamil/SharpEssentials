// Sharp Essentials
// Copyright 2016 Matthew Hamilton - matthamilton@live.com
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
using System.Windows.Input;

namespace SharpEssentials.Controls.Mvvm.Commands.Builder
{
	/// <summary>
	/// Relay command whose ability to execute can be externally triggered.
	/// </summary>
	internal class TriggeredRelayCommand<T> : RelayCommand<T>, ITriggerableCommand
    {
        /// <summary>
        /// Initializes a new <see cref="TriggeredRelayCommand{T}"/>.
        /// </summary>
        /// <param name="execute">The operation to execute.</param>
        /// <param name="canExecute">Determines whether a command can execute.</param>
	    public TriggeredRelayCommand(Action<T> execute, Predicate<T> canExecute) 
            : base(execute, canExecute)
	    {
	    }

	    /// <summary>
		/// Raises the <see cref="ICommand.CanExecuteChanged"/> event.
		/// </summary>
		public void RaiseCanExecuteChanged()
		{
            OnCanExecuteChanged();
		}

	    /// <see cref="ICommand.CanExecuteChanged"/>
	    public override event EventHandler CanExecuteChanged;

        /// <summary>
		/// Raises the <see cref="ICommand.CanExecuteChanged"/> event.
		/// </summary>
        protected override void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}