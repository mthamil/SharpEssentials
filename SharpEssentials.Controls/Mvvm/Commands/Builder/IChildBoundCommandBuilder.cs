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
using System.ComponentModel;
using System.Linq.Expressions;

namespace SharpEssentials.Controls.Mvvm.Commands.Builder
{
    /// <summary>
    /// Creates commands whose ability to execute depends on properties of a child collection's elements.
    /// </summary>
    /// <typeparam name="TParent">The type of parent that owns the child collection.</typeparam>
	/// <typeparam name="TChild">The type of object whose properties the parent depends on.</typeparam>
    public interface IChildBoundCommandBuilder<TParent, TChild> where TChild : INotifyPropertyChanged
    {
        /// <summary>
		/// Defines a condition using a property of the child objects that, when true, allows a command to execute.
		/// </summary>
		/// <param name="aggregatePredicate">
		/// The condition that determines whether a command can execute. This predicate
		/// must make use of a property of a child object.
		/// </param>
		/// <returns>A builder that allows specification of the command operation</returns>
        ICommandCompleter When(Expression<Func<IEnumerable<TChild>, bool>> aggregatePredicate);

        /// <summary>
		/// Specifies that a parent property of the object that owns a command determines whether it can execute.
		/// </summary>
		/// <param name="parentProperty">The property that determines whether a command can execute</param>
		/// <returns>A builder that enables setting the child property the parent property's value depends on</returns>
        IDependentChildPropertyCommandBuilder<TChild> Where(Expression<Func<TParent, bool>> parentProperty);
    }
}