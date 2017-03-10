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

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

[assembly: AssemblyProduct("SharpEssentials.Controls")]
[assembly: AssemblyCopyright("Copyright © Matt Hamilton 2017")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.*")]
[assembly: AssemblyInformationalVersion("1.0")]

[assembly: AssemblyTitle("SharpEssentials.Controls")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: ThemeInfo(
	ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
	//(used if a resource is not found in the page, 
	// or application resource dictionaries)
	ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
	//(used if a resource is not found in the page, 
	// app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsPrefix("https://github.com/mthamil/SharpEssentials/xaml", "se")]
[assembly: XmlnsDefinition("https://github.com/mthamil/SharpEssentials/xaml", "SharpEssentials.Controls")]
[assembly: XmlnsDefinition("https://github.com/mthamil/SharpEssentials/xaml", "SharpEssentials.Controls.Behaviors")]
[assembly: XmlnsDefinition("https://github.com/mthamil/SharpEssentials/xaml", "SharpEssentials.Controls.Behaviors.Clickable")]
[assembly: XmlnsDefinition("https://github.com/mthamil/SharpEssentials/xaml", "SharpEssentials.Controls.Behaviors.GridSplitterExpanderSupport")]
[assembly: XmlnsDefinition("https://github.com/mthamil/SharpEssentials/xaml", "SharpEssentials.Controls.Behaviors.Interactivity")]
[assembly: XmlnsDefinition("https://github.com/mthamil/SharpEssentials/xaml", "SharpEssentials.Controls.Commands")]
[assembly: XmlnsDefinition("https://github.com/mthamil/SharpEssentials/xaml", "SharpEssentials.Controls.Converters")]
[assembly: XmlnsDefinition("https://github.com/mthamil/SharpEssentials/xaml", "SharpEssentials.Controls.Markup")]
[assembly: XmlnsDefinition("https://github.com/mthamil/SharpEssentials/xaml", "SharpEssentials.Controls.MultiKey")]
[assembly: XmlnsDefinition("https://github.com/mthamil/SharpEssentials/xaml", "SharpEssentials.Controls.Selectors")]

[assembly: InternalsVisibleTo("SharpEssentials.Tests.Unit")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
