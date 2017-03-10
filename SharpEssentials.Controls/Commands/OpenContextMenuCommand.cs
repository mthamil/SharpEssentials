﻿// Sharp Essentials
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

using System.Windows;
using SharpEssentials.Controls.Mvvm.Commands;

namespace SharpEssentials.Controls.Commands
{
	/// <summary>
	/// Command that opens a context menu.
	/// </summary>
	public class OpenContextMenuCommand : RelayCommand<FrameworkElement>
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		public OpenContextMenuCommand()
			: base(OpenContextMenu) { }

		private static void OpenContextMenu(FrameworkElement element)
		{
			element.ContextMenu.PlacementTarget = element;
			element.ContextMenu.IsOpen = true;
		}
	}
}