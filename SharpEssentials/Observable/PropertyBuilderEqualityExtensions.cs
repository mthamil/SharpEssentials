// Sharp Essentials
// Copyright 2014 Matthew Hamilton - matthamilton@live.com
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
// 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpEssentials.InputOutput;

namespace SharpEssentials.Observable
{
    /// <summary>
    /// Provides extensions for <see cref="PropertyBuilder{T,V}"/> that define custom
    /// equality comparisons used to determine when a property's value has changed.
    /// </summary>
    public static class PropertyBuilderEqualityExtensions
    {
        /// <summary>
        /// Specifies string comparison options.
        /// </summary>
        public static PropertyBuilder<T, string> UsingStringEquality<T>(this PropertyBuilder<T, string> builder, StringComparison comparisonOption)
        {
            return builder.EqualWhen((x, y) => String.Equals(x, y, comparisonOption));
        }

        /// <summary>
        /// Specifies that the value for a collection property changes when the items in the collection change.
        /// </summary>
        public static PropertyBuilder<T, TCollection> UsingSequenceEquality<T, TCollection, TItem>(this PropertyBuilder<T, TCollection> builder, IEqualityComparer<TItem> itemComparer = null) 
                where TCollection : IEnumerable<TItem>
        {
            if (itemComparer == null)
                itemComparer = EqualityComparer<TItem>.Default;

            return builder.EqualWhen((x, y) => SequencesEqual(x, y, itemComparer));
        }

        private static bool SequencesEqual<TCollection, TItem>(TCollection x, TCollection y, IEqualityComparer<TItem> itemComparer) where TCollection : IEnumerable<TItem>
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            return x.SequenceEqual(y, itemComparer);
        }

        /// <summary>
        /// Specifies that the value for a <see cref="FileSystemInfo"/> property changes when its full path changes.
        /// </summary>
        public static PropertyBuilder<T, TFileSystemInfo> UsingPathEquality<T, TFileSystemInfo>(this PropertyBuilder<T, TFileSystemInfo> builder)
                where TFileSystemInfo : FileSystemInfo
        {
            return builder.EqualWhen(FileSystemInfoPathEqualityComparer.Instance.Equals);
        }
    }
}