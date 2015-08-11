﻿using System;
using System.Threading;
using System.Threading.Tasks;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Controls.Mvvm.Commands;
using SharpEssentials.Observable;
using SharpEssentials.Testing;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Mvvm.Commands
{
	public class BoundRelayCommandTests
	{
		[Fact]
		public void Test_Execute()
		{
			// Arrange.
			var propertyOwner = new TestClass();

			bool executed = false;
			var command = Command.For(propertyOwner)
			                     .DependsOn(p => p.BoolValue)
			                     .Executes(() => executed = true);

			// Act.
			command.Execute(null);

			// Assert.
			Assert.True(executed);
		}

		[Fact]
		public void Test_Execute_Asynchronously()
		{
			// Arrange.
			using (var resetEvent = new ManualResetEventSlim())
			{
				var propertyOwner = new TestClass();

				var command = Command.For(propertyOwner)
				                     .DependsOn(p => p.BoolValue)
				                     .Asynchronously()
				                     .Executes(async () =>
				                     {
					                     await Task.Delay(TimeSpan.FromMilliseconds(100));
					                     resetEvent.Set();
				                     });

				// Act.
				command.Execute(null);

				// Assert.
				resetEvent.Wait();
				Assert.True(resetEvent.IsSet);
			}
		}

		[Fact]
		public void Test_CanExecute()
		{
			foreach (var value in new [] { true, false })
			{
				// Arrange.
				var propertyOwner = new TestClass();
				var command = Command.For(propertyOwner)
				                     .DependsOn(p => p.BoolValue)
				                     .Executes(() => { });
				propertyOwner.BoolValue = value;

				// Act.
				bool canExecute = command.CanExecute(null);

				// Assert.
				Assert.Equal(value, canExecute);
			}
		}

		[Fact]
		public void Test_CanExecuteChanged()
		{
			// Arrange.
			var propertyOwner = new TestClass();
			var command = Command.For(propertyOwner)
			                     .DependsOn(p => p.BoolValue)
			                     .Executes(() => { });

			// Act/Assert.
			AssertThat.Raises(command, c => c.CanExecuteChanged += null, () => propertyOwner.BoolValue = true);
		}

		[Fact]
		public void Test_CanExecuteChanged_DifferentProperty()
		{
			// Arrange.
			var propertyOwner = new TestClass();
			var command = Command.For(propertyOwner)
			                     .DependsOn(p => p.BoolValue)
			                     .Executes(() => { });

			// Act/Assert.
			AssertThat.DoesNotRaise(command, c => c.CanExecuteChanged += null, () => propertyOwner.BoolValue2 = true);
		}

		private class TestClass : ViewModelBase
		{
			public TestClass()
			{
				_boolValue = Property.New(this, p => BoolValue, OnPropertyChanged);
				_boolValue2 = Property.New(this, p => BoolValue2, OnPropertyChanged);
			}

			public bool BoolValue
			{
				get { return _boolValue.Value; }
				set { _boolValue.Value = value; }
			}

			public bool BoolValue2
			{
				get { return _boolValue2.Value; }
				set { _boolValue2.Value = value; }
			}

			private readonly Property<bool> _boolValue;
			private readonly Property<bool> _boolValue2;
		}
	}
}