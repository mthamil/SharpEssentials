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
using SharpEssentials.Controls.Commands;
using Xunit;

namespace Tests.Unit.SharpEssentials.Controls.Commands
{
	public class CloseWindowCommandTests
	{
		[Fact]
		public void Test_CanExecute()
		{
			// Act.
			var canExecute = _command.CanExecute(null);

			// Assert.
			Assert.True(canExecute);
		}

		[Fact]
		public void Test_Execute()
		{
			// Arrange.
			var window = new Window();

			bool closed = false;
			window.Closed += (o, e) => closed = true;

			// Act.
			_command.Execute(window);

			// Assert.
			Assert.True(closed);
		}

		private readonly CloseWindowCommand _command = new CloseWindowCommand();
	}
}