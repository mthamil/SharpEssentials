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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SharpEssentials.Controls.Behaviors
{
    /// <summary>
    /// An attached behavior that provides a bindable collection containing the nodes in the path to the selected item in
    /// a <see cref="TreeView"/>.
    /// </summary>
    public class SelectedItemPath : LoadDependentBehavior<TreeView>
    {
        /// <summary>
        /// A <see cref="TreeView"/>'s selected item path.
        /// </summary>
        public IReadOnlyCollection<object> ItemPath
        {
            get { return (IReadOnlyCollection<object>)GetValue(ItemPathProperty); }
            set { SetValue(ItemPathProperty, value); }
        }

        /// <summary>
        /// The <see cref="ItemPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemPathProperty = 
            DependencyProperty.Register(nameof(ItemPath), 
                typeof(IReadOnlyCollection<object>), 
                typeof(SelectedItemPath), 
                new PropertyMetadata(new object[0]));

        /// <see cref="LoadDependentBehavior{T}.OnLoaded"/>
        protected override void OnLoaded()
        {
            AssociatedObject.SelectedItemChanged += AssociatedObject_SelectedItemChanged;
        }

        private void AssociatedObject_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null)
            {
                ItemPath = new object[0];
                return;
            }

            ItemPath = AssociatedObject.GetItemPath(e.NewValue);
        }
    }
}