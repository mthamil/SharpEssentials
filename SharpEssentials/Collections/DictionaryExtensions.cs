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

using System.Collections.Generic;

namespace SharpEssentials.Collections
{
	/// <summary>
	/// Contains extension methods for dictionaries.
	/// </summary>
	public static class DictionaryExtensions
	{
	    /// <summary>
	    /// Attempts to get the value associated with the specified key.
	    /// </summary>
	    /// <typeparam name="TKey">The type of key</typeparam>
	    /// <typeparam name="TValue">The type of value</typeparam>
	    /// <param name="dictionary">The dictionary to query</param>
	    /// <param name="key">The key whose value to get</param>
	    /// <returns>Option&lt;TValue&gt;.Some() if the key exists, otherwise none</returns>
	    public static Option<TValue> TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) =>
	        dictionary.TryGetValue(key, out TValue value)
	            ? Option.Some(value)
	            : Option.None<TValue>();
	}
}