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

using System.Linq;
using System.Windows.Controls;

namespace SharpEssentials.Controls
{
	/// <summary>
	/// Contains extension methods for <see cref="TreeView"/>s.
	/// </summary>
	public static class TreeViewExtensions
	{
		/// <summary>
		/// Performs a depth-first search of the already generated items in a tree for a given item's
		/// corresponding <see cref="TreeViewItem"/>.
		/// </summary>
		/// <param name="treeView">The tree to search</param>
		/// <param name="item">The object to search for</param>
		/// <returns>The corresponding <see cref="TreeViewItem"/> or <see cref="Option{TreeViewItem}.None"/></returns>
		public static Option<TreeViewItem> FindContainerFromItem(this TreeView treeView, object item)
		{
			var found = FindGeneratedItem(treeView.ItemContainerGenerator, item);
			return Option.From(found as TreeViewItem);
		}

		/// <summary>
		/// Attempts to find an item in a tree.
		/// </summary>
		private static object FindGeneratedItem(ItemContainerGenerator containerGenerator, object item)
		{
			var container = containerGenerator.ContainerFromItem(item);
			if (container != null)
				return container;

		    return containerGenerator.Items
		                             .Select(containerGenerator.ContainerFromItem)
		                             .Where(c => c != null)
		                             .OfType<TreeViewItem>()
		                             .Select(c => FindGeneratedItem(c.ItemContainerGenerator, item))
		                             .FirstOrDefault(foundItem => foundItem != null);
		}
	}
}