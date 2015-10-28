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