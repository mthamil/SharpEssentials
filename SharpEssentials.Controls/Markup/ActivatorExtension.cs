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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Markup;

namespace SharpEssentials.Controls.Markup
{
	/// <summary>
	/// A XAML markup extension to allow creating instances of types using
	/// generic type arguments and non-default constructors.
	/// </summary>
	[MarkupExtensionReturnType(typeof(object))]
	public class ActivatorExtension : MarkupExtension
	{
		/// <summary>
		/// Initializes the extension.
		/// </summary>
		public ActivatorExtension()
		{
			_instance = new Lazy<object>(() => 
				Activator.CreateInstance(ConstructType(), ConstructorArguments.ToArray()));
		}

		/// <summary>
		/// Initializes the extension with the type to instantiate.
		/// </summary>
		/// <param name="type">The type to instantiate</param>
		public ActivatorExtension(Type type)
			: this()
		{
			Type = type;
		}

		#region Overrides of MarkupExtension

		/// <see cref="MarkupExtension.ProvideValue"/>
		public override object ProvideValue(IServiceProvider serviceProvider) => _instance.Value;

	    #endregion

		/// <summary>
		/// The type to instantiate.
		/// </summary>
		public Type Type { get; set; }

		/// <summary>
		/// The type arguments of the type to instantiate.
		/// </summary>
		public Collection<Type> TypeArguments { get; } = new Collection<Type>();

	    /// <summary>
		/// The arguments to instantiate an instance with.
		/// </summary>
		public Collection<object> ConstructorArguments { get; } = new Collection<object>();

	    private Type ConstructType() =>
	        Type.IsGenericTypeDefinition
	            ? Type.MakeGenericType(TypeArguments.ToArray())
	            : Type;

	    private readonly Lazy<object> _instance;
	}
}