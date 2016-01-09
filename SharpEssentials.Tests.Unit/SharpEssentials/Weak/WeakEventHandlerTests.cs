using System;
using SharpEssentials.Weak;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Weak
{
	public class WeakEventHandlerTests
	{
		[Fact]
		public void Test_WeakEventHandler()
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
			new TestEventSubscriber(this, owner);
			return owner;
		}

		public class TestEventSubscriber
		{
			public TestEventSubscriber(WeakEventHandlerTests test, TestEventOwner owner)
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