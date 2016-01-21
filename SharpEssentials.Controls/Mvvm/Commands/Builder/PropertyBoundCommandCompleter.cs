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
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using SharpEssentials.Reflection;

namespace SharpEssentials.Controls.Mvvm.Commands.Builder
{
	/// <summary>
	/// Class that aids in creating a <see cref="ICommand"/> whose ability to execute
	/// is determined by a property.
	/// </summary>
	/// <typeparam name="TSource">The type of object for which a command is being built</typeparam>
	internal class PropertyBoundCommandCompleter<TSource> : BaseCommandCompleter where TSource : INotifyPropertyChanged
	{
		/// <summary>
		/// Initializes a new <see cref="PropertyBoundCommandCompleter{TSource}"/>.
		/// </summary>
		/// <param name="source">The object that declares the property the command is bound to</param>
		/// <param name="predicateProperty">The boolean property that determines whether a command can execute</param>
		public PropertyBoundCommandCompleter(TSource source, Expression<Func<TSource, bool>> predicateProperty)
		{
			_source = source;
		    _predicateProperty = new Lazy<Func<TSource, bool>>(predicateProperty.Compile);
		    _propertyName = Reflect.PropertyOf(predicateProperty).Name;
		}

		protected override Predicate<T> CanExecute<T>() => _ => _predicateProperty.Value(_source);

	    protected override TCommand Configure<TCommand>(TCommand command)
	    {
            _source.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == _propertyName)
                    command.RaiseCanExecuteChanged();
            };
            return command;
        }


        private readonly TSource _source;
	    private readonly Lazy<Func<TSource, bool>> _predicateProperty;
	    private readonly string _propertyName;
	}
}