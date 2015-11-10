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

namespace SharpEssentials
{
    /// <summary>
    /// A wrapper that can adapt objects to the <see cref="IDisposable"/> pattern if they don't 
    /// actually implement it.
    /// </summary>
    /// <typeparam name="T">The type of object to dispose of.</typeparam>
    public class DisposableAdapter<T> : DisposableBase where T : class
    {
        private readonly Action<T> _disposer;

        /// <summary>
        /// Initializes a new <see cref="DisposableAdapter{T}"/>.
        /// </summary>
        /// <param name="instance">The object to dispose.</param>
        /// <param name="disposer">The disposal action.</param>
        public DisposableAdapter(T instance, Action<T> disposer)
        {
            Value = instance;
            _disposer = disposer;
        }

        /// <summary>
        /// The wrapped object instance.
        /// </summary>
        public T Value { get; }

        /// <see cref="DisposableBase.OnDisposing"/>
        protected override void OnDisposing() => _disposer(Value);
    }

    public static class Dispose
    {
        /// <summary>
        /// Registers an object for disposal.
        /// </summary>
        /// <typeparam name="T">The type of object to dispose of.</typeparam>
        /// <param name="instance">An object that has some clean up behavior.</param>
        /// <param name="disposer">A mandatory clean up action.</param>
        public static DisposableAdapter<T> Of<T>(T instance, Action<T> disposer) where T : class
        {
            return new DisposableAdapter<T>(instance, disposer);
        }
    }
}