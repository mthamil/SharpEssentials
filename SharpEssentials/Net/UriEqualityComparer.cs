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

namespace SharpEssentials.Net
{
    /// <summary>
    /// An equality comparer that checks whether two URIs are equivalent disregarding case
    /// and ending slashes.
    /// </summary>
    public class UriEqualityComparer : IEqualityComparer<Uri>
    {
        /// <summary>
        /// Compares two URIs for equivalency.
        /// </summary>
        public bool Equals(Uri x, Uri y)
        {
            if (x == y)
                return true;

            if (x == null || y == null)
                return false;

            var xs = x.ToString().ToLowerInvariant().TrimEnd('/');
            var ys = y.ToString().ToLowerInvariant().TrimEnd('/');
            return xs == ys;
        }

        /// <summary>
        /// Gets a URI's hash code.
        /// </summary>
        public int GetHashCode(Uri obj) => obj?.ToString().ToLowerInvariant().TrimEnd('/').GetHashCode() ?? 0;

        /// <summary>
        /// Gets a <see cref="UriEqualityComparer"/> equality comparer.
        /// </summary>
        public static IEqualityComparer<Uri> Instance { get; } = new UriEqualityComparer();
    }
}