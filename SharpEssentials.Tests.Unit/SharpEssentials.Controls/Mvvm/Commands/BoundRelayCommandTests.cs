using System.Threading.Tasks;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Controls.Mvvm.Commands;
using SharpEssentials.Observable;
using SharpEssentials.Testing;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Controls.Mvvm.Commands
{
	public class BoundRelayCommandTests
	{
		[Fact]
		public void Test_Execute()
		{
            // Arrange.
            bool executed = false;
            var propertyOwner = new TestClass();

			var command = Command.For(propertyOwner)
			                     .DependsOn(p => p.BoolValue)
			                     .Executes(() => executed = true);

			// Act.
			command.Execute(null);

			// Assert.
			Assert.True(executed);
		}

        [Fact]
        public void Test_Execute_With_Parameter()
        {
            // Arrange.
            int result = 0;
            var propertyOwner = new TestClass();

            var command = Command.For(propertyOwner)
                                 .DependsOn(p => p.BoolValue)
                                 .Executes((int x) => result = x);

            // Act.
            command.Execute(2);

            // Assert.
            Assert.Equal(2, result);
        }

        [Fact]
		public async Task Test_Execute_Asynchronously()
		{
            // Arrange.
            bool executed = false;
            var propertyOwner = new TestClass();

            var command = Command.For(propertyOwner)
                                 .DependsOn(p => p.BoolValue)
                                 .ExecutesAsync(() =>
                                 {
                                     executed = true;
                                     return Task.CompletedTask;
                                 });

			// Act.
			await command.ExecuteAsync(null);

            // Assert.
            Assert.True(executed);
        }

        [Fact]
        public async Task Test_Execute_Asynchronously_With_Parameter()
        {
            // Arrange.
            int result = 0;
            var propertyOwner = new TestClass();

            var command = Command.For(propertyOwner)
                                 .DependsOn(p => p.BoolValue)
                                 .ExecutesAsync((int x) =>
                                 {
                                     result = x;
                                     return Task.CompletedTask;
                                 });

            // Act.
            await command.ExecuteAsync(2);

            // Assert.
            Assert.Equal(2, result);
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
