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
using System.Linq;
using System.Reflection;

namespace SharpEssentials.Reflection
{
    /// <summary>
    /// Contains extension methods for reflection types.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// If a type is a primitive such as int, returns its default, otherwise
        /// null is returned.
        /// </summary>
        /// <param name="type">The type to create a value for.</param>
        /// <returns>A default value for the type.</returns>
        public static object GetDefaultValue(this Type type)
        {
            if (type.GetTypeInfo().IsValueType && type != VoidType)	// can't create an instance of Void
                return Activator.CreateInstance(type);

            return null;
        }
        private static readonly Type VoidType = typeof(void);

        /// <summary>
        /// Checks whether another type is the generic type definition of this type.
        /// </summary>
        /// <param name="type">The closed type to check.</param>
        /// <param name="openGenericType">An open generic type that may be the definition of <paramref name="type"/>.</param>
        /// <returns>True if <paramref name="openGenericType"/> is the generic type definition of <paramref name="type"/>.</returns>
        public static bool IsClosedTypeOf(this Type type, Type openGenericType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (openGenericType == null)
                throw new ArgumentNullException(nameof(openGenericType));

            if (!openGenericType.GetTypeInfo().IsGenericTypeDefinition)
                throw new ArgumentException("Must be an open generic type.", nameof(openGenericType));

            return type.GetTypeInfo().IsGenericType && type.GetTypeInfo().GetGenericTypeDefinition() == openGenericType;
        }

        /// <summary>
        /// Searches for a declared constructor whose parameters match the specified types.
        /// </summary>
        /// <param name="type">The type to search.</param>
        /// <param name="parameters">The desired constructor parameters</param>
        /// <returns>A matching constructor or null if one is not found.</returns>
        public static ConstructorInfo GetDeclaredConstructor(this Type type, params Type[] parameters)
        {
            return type.GetTypeInfo()
                       .DeclaredConstructors
                       .SingleOrDefault(c => c.GetParameters().Select(p => p.ParameterType)
                                              .SequenceEqual(parameters));
        }
    }
}