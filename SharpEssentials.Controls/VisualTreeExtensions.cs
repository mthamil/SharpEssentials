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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace SharpEssentials.Controls
{
	/// <summary>
	/// Provides methods that help navigate visual trees.
	/// </summary>
	public static class VisualTreeExtensions
	{
		/// <summary>
		/// Returns an enumerable over a <see cref="DependencyObject"/>'s visual children.
		/// </summary>
		/// <param name="parent">The parent</param>
		/// <returns>An enumerable over immediate child visual tree elements</returns>
		public static IEnumerable<DependencyObject> VisualChildren(this DependencyObject parent)
		{
			if (parent == null)
				throw new ArgumentNullException(nameof(parent));

			return parent.EnumerateVisualTreeChildren();
		}

		private static IEnumerable<DependencyObject> EnumerateVisualTreeChildren(this DependencyObject parent)
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
				yield return VisualTreeHelper.GetChild(parent, i);
		}

        /// <summary>
        /// Recursively searches a <see cref="DependencyObject"/>'s visual tree
        /// for a child of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the child to search for.</typeparam>
        /// <param name="parent">The element at which to begin the search.</param>
        /// <returns>The desired element or <see cref="Option.None{T}"/> if it is not found.</returns>
	    public static Option<T> FindVisualChild<T>(this DependencyObject parent) where T : DependencyObject
        {
            return parent.FindVisualChild(dp => dp is T)
                         .Select(dp => (T)dp);
        }

	    /// <summary>
	    /// Recursively searches a <see cref="DependencyObject"/>'s visual tree
	    /// for a child that matches the given condition.
	    /// </summary>
	    /// <param name="parent">The element at which to begin the search.</param>
	    /// <param name="predicate">The condition that must be satisfied.</param>
	    /// <returns>The desired element or <see cref="Option.None{T}"/> if it is not found.</returns>
	    public static Option<DependencyObject> FindVisualChild(this DependencyObject parent, Func<DependencyObject, bool> predicate)
        {
            foreach (var visualChild in parent.VisualChildren())
            {
                var matches = predicate(visualChild);
                if (matches)
                    return visualChild;

                var found = visualChild.FindVisualChild(predicate);
                if (found.HasValue)
                    return found;
            }

            return Option.None<DependencyObject>();
        }
    }
}