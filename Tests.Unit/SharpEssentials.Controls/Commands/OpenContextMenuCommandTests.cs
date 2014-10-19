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
using SharpEssentials.Controls.Commands;
using Xunit;

namespace Tests.Unit.SharpEssentials.Controls.Commands
{
	public class OpenContextMenuCommandTests
	{
		[Fact]
		public void Test_Execute()
		{
			// Act.
			_command.Execute(_element);

			// Assert.
			Assert.Equal(_element, _element.ContextMenu.PlacementTarget);
			Assert.True(_element.ContextMenu.IsOpen);
		}

		private readonly OpenContextMenuCommand _command = new OpenContextMenuCommand();

		private readonly FrameworkElement _element = new FrameworkElement
		{
			ContextMenu = new ContextMenu()
		};
	}
}