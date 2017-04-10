using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using SharpEssentials.Controls;
using SharpEssentials.Testing;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Controls
{
    public class ItemsControlExtensionsTests
    {
        [WpfFact]
        public void Test_GenerateItems()
        {
            // Arrange.
            var nodes = new List<Node>
            {
                new Node("0")
                {
                    Children =
                    {
                        new Node("a"),
                        new Node("b")
                    }
                },
                new Node("1")
                {
                    Children =
                    {
                        new Node("c")
                    }
                }
            };

            PopulateTree(_underTest, nodes, new List<TreeViewItem>());

            // Act.
            _underTest.GenerateItems();

            // Assert.
            Assert.Equal(GeneratorStatus.ContainersGenerated, _underTest.ItemContainerGenerator.Status);
            foreach (var item in _underTest.Items)
            {
                var element = (ItemsControl)_underTest.ItemContainerGenerator.ContainerFromItem(item);
                Assert.NotNull(element);

                Assert.Equal(GeneratorStatus.ContainersGenerated, element.ItemContainerGenerator.Status);
                foreach (var child in element.Items)
                {
                    var childElement = element.ItemContainerGenerator.ContainerFromItem(child);
                    Assert.NotNull(childElement);
                }
            }
        }

        [WpfFact]
        public void Test_GetItemPath()
        {
            // Arrange.
            var nodes = new List<Node>
            {
                new Node("0")
                {
                    Children =
                    {
                        new Node("1"),
                        new Node("2")
                        {
                            Children =
                            {
                                new Node("a"),
                                new Node("b"),
                                new Node("c")
                            }
                        }
                    }
                }
            };

            var createdItems = new List<TreeViewItem>();
            PopulateTree(_underTest, nodes, createdItems);
            _underTest.GenerateItems();

            var targetItem = createdItems.Single(t => ((Node)t.DataContext).Name == "b");

            // Act.
            var path = _underTest.GetItemPath(targetItem);

            // Assert.
            AssertThat.SequenceEqual(new[] { "0", "2", "b" }, path.Cast<FrameworkElement>().Select(i => i.DataContext).Cast<Node>().Select(n => n.Name));
        }

        [WpfFact]
        public void Test_GetItems()
        {
            // Arrange.
            var nodes = new List<Node>
            {
                new Node("0"),
                new Node("1"),
                new Node("2"),
                new Node("3")
            };

            PopulateTree(_underTest, nodes, new List<TreeViewItem>());
            _underTest.GenerateItems();

            // Act.
            var items = _underTest.GetItems().Cast<FrameworkElement>();

            // Assert.
            AssertThat.SequenceEqual(new [] {"0", "1", "2", "3"}, items.Select(n => ((Node)n.DataContext).Name));
        }

        private static void PopulateTree<T>(ItemsControl tree, IEnumerable<Node> nodes, ICollection<T> collector) where T : ItemsControl, new()
        {
            foreach (var node in nodes)
            {
                var newItem = new T { DataContext = node };
                tree.Items.Add(newItem);
                collector.Add(newItem);
                PopulateTree(newItem, node.Children, collector);
            }
        }

        private readonly TreeView _underTest = new TreeView();

        [DebuggerDisplay("{Name}")]
        class Node
        {
            public Node(string name) {  Name = name; }
            public string Name { get; }
            public ICollection<Node> Children { get; } = new List<Node>();
        }
    }
}