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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace SharpEssentials.Controls.Behaviors
{
	/// <summary>
	/// Since the TreeView's SelectedItem property is readonly, it is not bindable.
	/// This attached behavior provides a selected item property that is bindable.
	/// </summary>
    public class TreeViewBindableSelectedItem : Behavior<TreeView>
	{
        /// <see cref="Behavior.OnAttached"/>
        protected override void OnAttached()
        {
            AssociatedObject.SelectedItemChanged += AssociatedObject_SelectedItemChanged;
        }

        /// <see cref="Behavior.OnDetaching"/>
        protected override void OnDetaching()
        {
            AssociatedObject.SelectedItemChanged -= AssociatedObject_SelectedItemChanged;
        }

        private void AssociatedObject_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue;
        }

        /// <summary>
        /// The currently selected tree item.a
        /// </summary>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// The SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem),
                typeof(object),
                typeof(TreeViewBindableSelectedItem),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var behavior = dependencyObject as TreeViewBindableSelectedItem;
            behavior?.AssociatedObject?.FindContainerFromItem(e.NewValue).Apply(item =>
            {
                if (item != null && !item.IsSelected)
                    item.IsSelected = true;
            });
        }
	}
}