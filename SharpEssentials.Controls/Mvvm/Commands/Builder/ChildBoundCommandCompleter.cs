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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using SharpEssentials.Reflection;
using static SharpEssentials.Weak.WeakEventManagerExtensions;

namespace SharpEssentials.Controls.Mvvm.Commands.Builder
{
	/// <summary>
	/// Class that completes construction of a command that depends on a child collection.
	/// </summary>
	/// <typeparam name="TChild">The type of child object the parent depends on</typeparam>
	public class ChildBoundCommandCompleter<TChild> : ICommandCompleter where TChild : INotifyPropertyChanged
	{
		/// <summary>
		/// Initializes a new <see cref="ChildBoundCommandCompleter{TChild}"/>.
		/// </summary>
		/// <param name="collectionGetter">Function that retrieves the collection whose items determine whether a command can execute</param>
		/// <param name="childProperty">A child property that the parent is somehow dependent upon for determining whether a command can execute</param>
		/// <param name="canExecute">The actual predicate that determines whether a command can execute</param>
		public ChildBoundCommandCompleter(
			Func<IEnumerable<TChild>> collectionGetter, 
			Expression<Func<TChild, bool>> childProperty,
			Func<bool> canExecute)
		{
			_collectionGetter = collectionGetter;
			_childProperty = childProperty;
			_canExecute = canExecute;
		}

		/// <summary>
		/// Sets the operation that a command will execute.
		/// </summary>
		/// <param name="operation">The parameterless operation to be executed</param>
		/// <returns>A new command</returns>
		public ICommand Executes(Action operation)
		{
			return Executes(_ => operation());
		}

		/// <summary>
		/// Sets the operation that a command will execute.
		/// </summary>
		/// <param name="operation">The operation to be executed</param>
		/// <returns>A new command</returns>
		public ICommand Executes(Action<object> operation)
		{
            var command = new TriggeredRelayCommand(operation, _ => _canExecute());

            var childPropertyName = Reflect.PropertyOf(_childProperty).Name;
            EventHandler<PropertyChangedEventArgs> propertyChangedHandler = (o, e) =>
		    {
                if (e.PropertyName == childPropertyName)
                {
                    if (_collectionGetter().Contains((TChild)o))
                        command.RaiseCanExecuteChanged();
                }
            };

		    var collection = _collectionGetter();
            var notifyingCollection = collection as INotifyCollectionChanged;
		    notifyingCollection?.AddWeakHandler<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>(nameof(INotifyCollectionChanged.CollectionChanged),
		        (o, e) =>
		        {
		            if (e.OldItems != null)
		            {
		                foreach (var removedItem in e.OldItems.Cast<INotifyPropertyChanged>())
		                    removedItem.RemoveWeakHandler(nameof(INotifyPropertyChanged.PropertyChanged), propertyChangedHandler);
		            }

		            if (e.NewItems != null)
		            {
		                foreach (var newItem in e.NewItems.Cast<INotifyPropertyChanged>())
		                    newItem.AddWeakHandler(nameof(INotifyPropertyChanged.PropertyChanged), propertyChangedHandler);
		            }
		        });

		    foreach (var existingChild in collection)
                existingChild.AddWeakHandler(nameof(INotifyPropertyChanged.PropertyChanged), propertyChangedHandler);

            return command;
		}

		private readonly Func<IEnumerable<TChild>> _collectionGetter;
		private readonly Expression<Func<TChild, bool>> _childProperty;
		private readonly Func<bool> _canExecute;
	}
}