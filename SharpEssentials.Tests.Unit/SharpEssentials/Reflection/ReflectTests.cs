﻿using System;
using System.Reflection;
using SharpEssentials.Reflection;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Reflection
{
	public class ReflectTests
	{
		[Fact]
		public void Test_PropertyOf_ValueType()
		{
			// Act.
			PropertyInfo property = Reflect.PropertyOf<TestType>(t => t.IntProperty);

			// Assert.
			Assert.Equal(nameof(TestType.IntProperty), property.Name);
			Assert.Equal(typeof(Int32), property.PropertyType);
			Assert.Equal(typeof(TestType), property.DeclaringType);
		}

		[Fact]
		public void Test_PropertyOf_ReferenceType()
		{
			// Act.
			PropertyInfo property = Reflect.PropertyOf<TestType>(t => t.StringProperty);

			// Assert.
			Assert.Equal(nameof(TestType.StringProperty), property.Name);
			Assert.Equal(typeof(String), property.PropertyType);
			Assert.Equal(typeof(TestType), property.DeclaringType);
		}

		[Fact]
		public void Test_PropertyOf_Method()
		{
			Assert.Throws<ArgumentException>(() => Reflect.PropertyOf<TestType>(t => t.ToString()));
		}

		[Fact]
		public void Test_PropertyOf_ConstantValue()
		{
			Assert.Throws<ArgumentException>(() => Reflect.PropertyOf<TestType>(t => 1));
		}

		[Fact]
		public void Test_MethodOf_HasReturnValue()
		{
			// Act.
			MethodInfo method = Reflect.MethodOf<TestType>(t => t.ToString());

			// Assert.
			Assert.Equal(nameof(TestType.ToString), method.Name);
			Assert.Equal(typeof(string), method.ReturnType);
			Assert.Equal(typeof(object), method.DeclaringType);
		}

		[Fact]
		public void Test_MethodOf_VoidReturn()
		{
			// Act.
			MethodInfo method = Reflect.MethodOf<TestType>(t => t.VoidReturning());

			// Assert.
			Assert.Equal(nameof(TestType.VoidReturning), method.Name);
			Assert.Equal(typeof(void), method.ReturnType);
			Assert.Equal(typeof(TestType), method.DeclaringType);
			Assert.Equal(0, method.GetParameters().Length);
		}

		[Fact]
		public void Test_MethodOf_Overload()
		{
			// Act.
			MethodInfo method = Reflect.MethodOf<TestType>(t => t.VoidReturning(0));

			// Assert.
			Assert.Equal(nameof(TestType.VoidReturning), method.Name);
			Assert.Equal(typeof(void), method.ReturnType);
			Assert.Equal(typeof(TestType), method.DeclaringType);
			Assert.Equal(1, method.GetParameters().Length);
		}

		[Fact]
		public void Test_MethodOf_Property()
		{
			Assert.Throws<ArgumentException>(() => Reflect.MethodOf<TestType>(t => t.StringProperty));
		}

		[Fact]
		public void Test_MethodOf_ConstantValue()
		{
			Assert.Throws<ArgumentException>(() => Reflect.MethodOf<TestType>(t => 1));
		}

		private class TestType
		{
			public int IntProperty { get; set; }
			public string StringProperty { get; set; }
			public void VoidReturning() {}
			public void VoidReturning(int value) { }
		}
	}
}