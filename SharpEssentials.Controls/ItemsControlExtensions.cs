using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SharpEssentials.Controls
{
    /// <summary>
    /// Contains extension methods for <see cref="ItemsControl"/>s.
    /// </summary>
    public static class ItemsControlExtensions
    {
        /// <summary>
        /// Searches a potentially nested <see cref="ItemsControl"/> (ie. a <see cref="TreeView"/>) for the given
        /// object and if found, returns a collection of the objects in the path to that object.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<object> GetItemPath(this ItemsControl items, object searchValue)
        {
            var path = new Stack<object>();
            GetItemPath(path, items, searchValue);
            return path;
        }

        private static bool GetItemPath(Stack<object> path, ItemsControl items, object searchValue)
        {
            foreach (var item in items.Items)
            {
                if (ReferenceEquals(item, searchValue))
                {
                    path.Push(item);
                    return true;
                }

                var node = items.ItemContainerGenerator.ContainerFromItem(item);
                var nodeItems = node as ItemsControl;
                if (nodeItems != null)
                {
                    var found = GetItemPath(path, nodeItems, searchValue);
                    if (found)
                    {
                        path.Push(item);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Recursively forces generation of an <see cref="ItemsControl"/>'s items.
        /// </summary>
        public static void GenerateItems(this ItemsControl tree)
        {
            var generator = (IItemContainerGenerator)tree.ItemContainerGenerator;
            using (generator.StartAt(new GeneratorPosition(-1, 0), GeneratorDirection.Forward))
            {
                ItemsControl next;
                do
                {
                    bool isNew;
                    next = generator.GenerateNext(out isNew) as ItemsControl;
                    next?.GenerateItems();
                } while (next != null);
            }
        }
    }
}