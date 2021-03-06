using System;
using SharpEssentials.Weak;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Weak
{
	public class WeakEventHandlerTests
	{
        [Fact]
        public void Test_WeakEventHandler_Subscribe()
        {
            // Arrange.
            var test = Setup();

            // Act.
            test.OnEvent();

            // Assert.
            Assert.True(test.HasSubscribers);
        }

        [Fact]
		public void Test_WeakEventHandler_Unsubscribes_When_Collected()
		{
			// Arrange.
			var test = Setup();
                
			// Act.
			GC.Collect();
			GC.WaitForPendingFinalizers();
			test.OnEvent();

			// Assert.
			Assert.False(test.HasSubscribers);
		}

		private TestEventOwner Setup()
		{
			var owner = new TestEventOwner();
			new TestEventSubscriber(owner);
			return owner;
		}

		public class TestEventSubscriber
		{
			public TestEventSubscriber(TestEventOwner owner)
			{
				owner.Event += new EventHandler<EventArgs>(owner_Event)
					.MakeWeak(eh => { owner.Event -= eh; });
			}

			private void owner_Event(object sender, EventArgs e) { }
		}

		public class TestEventOwner
		{
			public event EventHandler<EventArgs> Event;

			public void OnEvent()
			{
			    Event?.Invoke(this, EventArgs.Empty);
			}

		    public bool HasSubscribers => Event != null;
		}
	}
}