using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SharpEssentials.Controls.Behaviors
{
    /// <summary>
    /// Behavior that enables dragging items to reorder them.
    /// </summary>
    public class ItemDragReordering : Behavior<ItemsControl>
    {
        /// <see cref="Behavior.OnAttached"/>
        protected override void OnAttached()
        {
            AssociatedObject.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (AssociatedObject.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Wire up drag events for new items.
                    var items = AssociatedObject.GetItems()
                                                .OfType<UIElement>()
                                                .Where(i => !i.AllowDrop);
                    foreach (var item in items)
                    {
                        item.AllowDrop = true;
                        item.PreviewMouseLeftButtonDown += Item_PreviewMouseLeftButtonDown;
                        item.PreviewMouseMove += Item_PreviewMouseMove;
                        item.DragEnter += Item_DragEnter;
                        item.Drop += Item_Drop;
                    }
                }));
            }
        }

        private void Item_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _start = e.GetPosition(null);   // Record mouse start position.
        }

        private void Item_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Determine that the distance the mouse moved is greater than a given threshold.
                // This avoids false drags on non-moving simple clicks.
                var mousePos = e.GetPosition(null);
                var moveDistance = _start - mousePos;

                if (Math.Abs(moveDistance.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(moveDistance.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    var item = (DependencyObject)sender;
                    var itemData = AssociatedObject.ItemContainerGenerator.ItemFromContainer(item);
                    DragDrop.DoDragDrop(item, itemData, DragDropEffects.Move);
                }
            }
        }

        private void Item_DragEnter(object sender, DragEventArgs e)
        {
            var targetItem = AssociatedObject.ItemContainerGenerator.ItemFromContainer((DependencyObject)sender);
            object sourceItem;
            if (!e.Data.TryGetData(targetItem.GetType(), out sourceItem) || Object.ReferenceEquals(sourceItem, targetItem))
                e.Effects = DragDropEffects.None;
        }

        private void Item_Drop(object sender, DragEventArgs e)
        {
            var targetItem = AssociatedObject.ItemContainerGenerator.ItemFromContainer((DependencyObject)sender);
            object sourceItem;
            if (e.Data.TryGetData(targetItem.GetType(), out sourceItem) && !Object.ReferenceEquals(sourceItem, targetItem))
            {
                var targetIndex = AssociatedObject.ItemContainerGenerator.IndexFromContainer((DependencyObject)sender);
                var items = AssociatedObject.ItemsSource as IList;
                if (items != null)
                {
                    items.Remove(sourceItem);
                    items.Insert(targetIndex, sourceItem);
                    var selector = AssociatedObject as Selector;
                    if (selector != null)
                        selector.SelectedItem = sourceItem;
                }
            }
        }

        private Point _start;
    }

    static class DataObjectExtensions
    {
        public static bool TryGetData(this IDataObject obj, Type format, out object data)
        {
            if (obj.GetDataPresent(format))
            {
                data = obj.GetData(format);
                return true;
            }

            data = null;
            return false;
        }
    }
}