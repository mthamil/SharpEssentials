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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace SharpEssentials.Reflection
{
	/// <summary>
	/// Contains methods used to extract reflection information in a type-safe manner.
	/// </summary>
	public static class Reflect
	{
		/// <summary>
		/// Extracts the PropertyInfo for the property referred to by the given lambda expression.
		/// </summary>
		/// <remarks>This is usually the most convenient overload to use because it requires the fewest generic arguments.</remarks>
		/// <typeparam name="TDeclaring">The type that should contain the property</typeparam>
		/// <param name="propertyAccessor">A lambda expression referring to the property</param>
		/// <returns>The PropertyInfo of the given property</returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static PropertyInfo PropertyOf<TDeclaring>(Expression<Func<TDeclaring, object>> propertyAccessor)
		{
			return PropertyOf<TDeclaring, object>(propertyAccessor);
		}

		/// <summary>
		/// Extracts the PropertyInfo for the property referred to by the given lambda expression.
		/// </summary>
		/// <typeparam name="TDeclaring">The type that should contain the property</typeparam>
		/// <typeparam name="TValue">The type of the property's value</typeparam>
		/// <param name="propertyAccessor">A lambda expression referring to the property</param>
		/// <returns>The PropertyInfo of the given property</returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static PropertyInfo PropertyOf<TDeclaring, TValue>(Expression<Func<TDeclaring, TValue>> propertyAccessor)
		{
			return PropertyOf(typeof(TDeclaring), propertyAccessor);
		}

		/// <summary>
		/// Extracts the PropertyInfo for the property referred to by the given lambda expression.
		/// </summary>
		/// <param name="declaring">The declaring type</param>
		/// <param name="propertyAccessor">A lambda expression referring to the property</param>
		/// <returns>The PropertyInfo of the given property</returns>
		public static PropertyInfo PropertyOf(Type declaring, LambdaExpression propertyAccessor)
		{
			return ExtractProperty(declaring, propertyAccessor);
		}

		private static PropertyInfo ExtractProperty(Type type, LambdaExpression propertyAccessor)
		{
			var memberExpr = ExtractMemberExpression(type, propertyAccessor);
			if (memberExpr == null)
				throw new ArgumentException($"The body of the expression must be a member of {type.Name}", nameof(propertyAccessor));

			var property = memberExpr.Member as PropertyInfo;
			if (property == null)
				throw new ArgumentException("The body of the expression must be a property invocation.", nameof(propertyAccessor));

			return property;
		}

		private static MemberExpression ExtractMemberExpression(Type type, LambdaExpression memberAccessor)
		{
		    switch (memberAccessor.Body)
		    {
                case MemberExpression memberExpr:
                    return memberExpr;

                case UnaryExpression unaryExpr when unaryExpr.NodeType == ExpressionType.Convert:
                    if (unaryExpr.Operand is MemberExpression unaryOperand &&
                        type.GetTypeInfo().IsAssignableFrom(unaryOperand.Member.DeclaringType.GetTypeInfo()))
                    {
                        return unaryOperand;
                    }

                    goto default;

                default:
                    return null;
            }
		}

		/// <summary>
		/// Extracts a void-returning method from a lambda expression.
		/// </summary>
		/// <typeparam name="T">The declaring type of the method</typeparam>
		/// <param name="methodCaller">An expression that invokes the method</param>
		/// <returns>The MethodInfo of the given method</returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static MethodInfo MethodOf<T>(this Expression<Action<T>> methodCaller)
		{
			return ExtractMethodCall(typeof(T), methodCaller).Method;
		}

		/// <summary>
		/// Extracts a method with a return type from a lambda expression.
		/// </summary>
		/// <typeparam name="T">The declaring type of the method</typeparam>
		/// <param name="methodCaller">The expression that invokes the method</param>
		/// <returns>The MethodInfo of the given method</returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static MethodInfo MethodOf<T>(this Expression<Func<T, object>> methodCaller)
		{
			return ExtractMethodCall(typeof(T), methodCaller).Method;
		}

		private static MethodCallExpression ExtractMethodCall(Type type, LambdaExpression methodCaller)
		{
		    if (methodCaller.Body is MethodCallExpression methodCall)
		        return methodCall;

			throw new ArgumentException($"The body of the expression must be a method of {type.Name}", nameof(methodCaller));
		}
	}
}
