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
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using SharpEssentials.Controls.Rendering;

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

        /// <see cref="Behavior.OnDetaching"/>
        protected override void OnDetaching()
        {
            AssociatedObject.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
        }

        /// <summary>
        /// Whether to use a ghosted image of the item being dragged as the cursor.
        /// </summary>
        public bool ShowItemBeingDragged
        {
            get => (bool)GetValue(ShowItemBeingDraggedProperty);
            set => SetValue(ShowItemBeingDraggedProperty, value);
        }

        /// <summary>
        /// Dependency property for the <see cref="ShowItemBeingDragged"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowItemBeingDraggedProperty = 
            DependencyProperty.Register(
                nameof(ShowItemBeingDragged), 
                typeof(bool), 
                typeof(ItemDragReordering), 
                new PropertyMetadata(false));

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
                        item.GiveFeedback += Item_GiveFeedback;
                        item.PreviewMouseLeftButtonDown += Item_PreviewMouseLeftButtonDown;
                        item.PreviewMouseMove += Item_PreviewMouseMove;
                        item.DragEnter += Item_DragEnter;
                        item.Drop += Item_Drop;
                    }
                }));
            }
        }

        private void Item_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (!ShowItemBeingDragged)
                return;

            if (e.Effects.HasFlag(DragDropEffects.Move))
            {
                e.UseDefaultCursors = false;
                Mouse.SetCursor(((UIElement)e.Source).ToCursor(opacity: 0.75));
            }
            else
            {
                e.UseDefaultCursors = true;
            }

            e.Handled = true;
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
            if (!e.Data.TryGetData(targetItem.GetType(), out var sourceItem) || Object.ReferenceEquals(sourceItem, targetItem))
                e.Effects = DragDropEffects.None;
        }

        private void Item_Drop(object sender, DragEventArgs e)
        {
            var targetItem = AssociatedObject.ItemContainerGenerator.ItemFromContainer((DependencyObject)sender);
            if (e.Data.TryGetData(targetItem.GetType(), out var sourceItem) && !Object.ReferenceEquals(sourceItem, targetItem))
            {
                var targetIndex = AssociatedObject.ItemContainerGenerator.IndexFromContainer((DependencyObject)sender);
                var items = AssociatedObject.ItemsSource as IList;
                if (items != null)
                {
                    items.Remove(sourceItem);
                    items.Insert(targetIndex, sourceItem);
                    if (AssociatedObject is Selector selector)
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