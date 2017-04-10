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

namespace SharpEssentials.Weak
{
    /// <summary>
    /// Provides extension methods for <see cref="WeakReference{T}"/>s
    /// </summary>
    public static class WeakReferenceExtensions
    {
        /// <summary>
        /// Tries to retrieve the target object that is referenced by the current <see cref="T:System.WeakReference{T}"/> object.
        /// </summary>
        public static Option<T> TryGetTarget<T>(this WeakReference<T> reference) where T : class =>
            reference.TryGetTarget(out T target)
                ? target
                : Option.None<T>();
    }
}