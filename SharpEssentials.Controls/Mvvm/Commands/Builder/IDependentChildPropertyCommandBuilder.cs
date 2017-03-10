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
using System.ComponentModel;
using System.Linq.Expressions;

namespace SharpEssentials.Controls.Mvvm.Commands.Builder
{
    /// <summary>
    /// Allows specification of a child property that a parent property's value depends on.
    /// </summary>
    /// <typeparam name="TChild">The type of the child objects that the parent depends on.</typeparam>
    public interface IDependentChildPropertyCommandBuilder<TChild> where TChild : INotifyPropertyChanged
    {
        /// <summary>
        /// Specifies the child property that the parent property's value depends on.
        /// </summary>
        /// <param name="childProperty">A child property that the parent property's value depends on</param>
        /// <returns>A builder that allows specification of the command operation</returns>
        ICommandCompleter DependsOn(Expression<Func<TChild, bool>> childProperty);
    }
}