using System;
using System.Threading;
using System.Threading.Tasks;
using SharpEssentials.Concurrency;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Concurrency
{
	public class TaskExtensionsTests
	{
		[Fact]
		public void Test_Then_WithResults_BothComplete()
		{
			// Act.
			Task<int> task = 
				Task.Factory.StartNew(() => 1m, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(result => Task<int>.Factory.StartNew(() => (int)result + 1, _cts.Token, TaskCreationOptions.None, _taskScheduler));

			task.Wait();

			// Assert.
			Assert.Equal(2, task.Result);
			Assert.Equal(TaskStatus.RanToCompletion, task.Status);
		}

		[Fact]
		public void Test_Then_WithResults_FirstCompletes_SecondFails()
		{
			// Act.
			Task<int> task = 
				Task.Factory.StartNew(() => 1m, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(result => Task<int>.Factory.StartNew(() => throw new InvalidOperationException(), _cts.Token, TaskCreationOptions.None, _taskScheduler));


			// Assert.
			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsFaulted);
			Assert.NotNull(task.Exception);
			Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithResults_FirstCompletes_SecondFailsCreation()
		{
			// Act.
			Task<int> task = 
				Task.Factory.StartNew(() => 1m, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(result => (Task<int>)null);


			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithResults_FirstCompletes_SecondCanceled()
		{
			// Act.
			Task<int> task =
				Task.Factory.StartNew(() => 1m, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(result => Task<int>.Factory.StartNew(() =>
					{
						_cts.Cancel();
						_cts.Token.ThrowIfCancellationRequested();
						return 1;
					}, _cts.Token, TaskCreationOptions.None, _taskScheduler));

			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithResults_FirstFails()
		{
			// Act.
			Task<int> task = 
				Task<decimal>.Factory.StartNew(() => throw new InvalidOperationException(), _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(result => Task<int>.Factory.StartNew(() => (int)result + 1, _cts.Token, TaskCreationOptions.None, _taskScheduler));


			// Assert.
			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsFaulted);
			Assert.NotNull(task.Exception);
			Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithResults_FirstCanceled()
		{
			// Act.
			Task<int> task = 
				Task.Factory.StartNew(() =>
				{
					_cts.Cancel();
					_cts.Token.ThrowIfCancellationRequested();
					return 1m;
				}, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(result => Task<int>.Factory.StartNew(() => (int)result + 1, _cts.Token, TaskCreationOptions.None, _taskScheduler));

			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithoutResults_BothComplete()
		{
			// Act.
			Task task = 
				Task.Factory.StartNew(() => { }, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(() => Task.Factory.StartNew(() => { }, _cts.Token, TaskCreationOptions.None, _taskScheduler));

			task.Wait();

			// Assert.
			Assert.Equal(TaskStatus.RanToCompletion, task.Status);
		}

		[Fact]
		public void Test_Then_WithoutResults_FirstCompletes_SecondFails()
		{
			// Act.
			Task task = 
				Task.Factory.StartNew(() => { }, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(() => Task.Factory.StartNew(() => throw new InvalidOperationException(), _cts.Token, TaskCreationOptions.None, _taskScheduler));


			// Assert.
			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsFaulted);
			Assert.NotNull(task.Exception);
			Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithoutResults_FirstCompletes_SecondFailsCreation()
		{
			// Act.
			Task task = 
				Task.Factory.StartNew(() => 1m, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(() => (Task)null);


			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithoutResults_FirstCompletes_SecondCanceled()
		{
			// Act.
			Task task = 
				Task.Factory.StartNew(() => 1m, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(() => Task.Factory.StartNew(() =>
					{
						_cts.Cancel();
						_cts.Token.ThrowIfCancellationRequested();;
					}, _cts.Token, TaskCreationOptions.None, _taskScheduler));

			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithoutResults_FirstFails()
		{
			// Act.
			Task task = 
				Task<decimal>.Factory.StartNew(() => throw new InvalidOperationException(), _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(() => Task.Factory.StartNew(() => { }, _cts.Token, TaskCreationOptions.None, _taskScheduler));


			// Assert.
			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsFaulted);
			Assert.NotNull(task.Exception);
			Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithoutResults_FirstCanceled()
		{
			// Act.
			Task task = 
				Task.Factory.StartNew(() =>
				{
					_cts.Cancel();
					_cts.Token.ThrowIfCancellationRequested();
				}, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(() => Task.Factory.StartNew(() => { }, _cts.Token, TaskCreationOptions.None, _taskScheduler));

			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_OnlySecondHasResults_BothComplete()
		{
			// Act.
			Task<int> task = 
				Task.Factory.StartNew(() => { }, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(() => Task<int>.Factory.StartNew(() => 1, _cts.Token, TaskCreationOptions.None, _taskScheduler));

			task.Wait();

			// Assert.
			Assert.Equal(1, task.Result);
			Assert.Equal(TaskStatus.RanToCompletion, task.Status);
		}

		[Fact]
		public void Test_Then_OnlySecondHasResults_FirstCompletes_SecondFails()
		{
			// Act.
			Task<int> task =
				Task.Factory.StartNew(() => { }, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(() => Task<int>.Factory.StartNew(() => throw new InvalidOperationException(), _cts.Token, TaskCreationOptions.None, _taskScheduler));

			// Assert.
			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsFaulted);
			Assert.NotNull(task.Exception);
			Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
		}

		[Fact]
		public void Test_Then_OnlySecondHasResults_FirstCompletes_SecondFailsCreation()
		{
			// Act.
			Task<int> task =
				Task.Factory.StartNew(() => { }, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(() => (Task<int>)null);

			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_OnlySecondHasResults_FirstCompletes_SecondCanceled()
		{
			// Act.
			Task<int> task =
				Task.Factory.StartNew(() => { }, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(() => Task<int>.Factory.StartNew(() =>
					{
						_cts.Cancel();
						_cts.Token.ThrowIfCancellationRequested();
						return 1;
					}, _cts.Token, TaskCreationOptions.None, _taskScheduler));

			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_OnlySecondHasResults_FirstFails()
		{
			// Act.
			Task<int> task = 
				Task.Factory.StartNew(() => throw new InvalidOperationException(), _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(() => Task<int>.Factory.StartNew(() => 1, _cts.Token, TaskCreationOptions.None, _taskScheduler));


			// Assert.
			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsFaulted);
			Assert.NotNull(task.Exception);
			Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
		}

		[Fact]
		public void Test_Then_OnlySecondHasResults_FirstCanceled()
		{
			// Act.
			Task<int> task = 
				Task.Factory.StartNew(() =>
				{
					_cts.Cancel();
					_cts.Token.ThrowIfCancellationRequested();
				}, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(() => Task<int>.Factory.StartNew(() => 1, _cts.Token, TaskCreationOptions.None, _taskScheduler));

			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithResult_Continuation_BothComplete()
		{
			// Act.
			Task<string> task = 
				Task.Factory.StartNew(() => 1m, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(result => result.ToString());

			task.Wait();

			// Assert.
			Assert.Equal("1", task.Result);
		}

		[Fact]
		public void Test_Then_WithResult_Continuation_TaskFails()
		{
			// Act.
			Task<string> task =
				Task<int>.Factory.StartNew(() => throw new InvalidOperationException(), _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(result => result.ToString());

			// Assert.
			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsFaulted);
			Assert.NotNull(task.Exception);
			Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithResult_Continuation_TaskCancelled()
		{
			// Act.
			Task<string> task = 
				Task<decimal>.Factory.StartNew(() =>
				{
					_cts.Cancel();
					_cts.Token.ThrowIfCancellationRequested();
					return 1m;
				}, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(result => result.ToString());

			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithResult_Continuation_ContinuationFails()
		{
			// Act.
			Task<string> task = 
				Task.Factory.StartNew(() => 1m, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(new Func<decimal, string>(result => throw new InvalidOperationException()));

			// Assert.
			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsFaulted);
			Assert.NotNull(task.Exception);
			Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
		}

		[Fact]
		public void Test_Then_WithoutResult_Continuation_BothComplete()
		{
			// Arrange.
			bool continued = false;

			// Act.
			Task task =
				Task.Factory.StartNew(() => { }, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(() => continued = true);

			task.Wait();

			// Assert.
			Assert.True(continued);
		}

		[Fact]
		public void Test_Then_WithoutResult_Continuation_TaskFails()
		{
			// Arrange.
			bool continued = false;

			// Act.
			Task task =
				Task.Factory.StartNew(() => throw new InvalidOperationException(), _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(() => continued = true);

			// Assert.
			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsFaulted);
			Assert.NotNull(task.Exception);
			Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
			Assert.False(continued);
		}

		[Fact]
		public void Test_Then_WithoutResult_Continuation_TaskCancelled()
		{
			// Arrange.
			bool continued = false;

			// Act.
			Task task =
				Task.Factory.StartNew(() =>
				{
					_cts.Cancel();
					_cts.Token.ThrowIfCancellationRequested();
				}, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(() => continued = true);

			// Assert.
			var exception = Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsCanceled);
			Assert.IsType<TaskCanceledException>(exception.InnerException);
			Assert.False(continued);
		}

		[Fact]
		public void Test_Then_WithoutResult_Continuation_ContinuationFails()
		{
			// Act.
			Task task =
				Task.Factory.StartNew(() => { }, _cts.Token, TaskCreationOptions.None, _taskScheduler)
				.Then(new Action(() => throw new InvalidOperationException()));

			// Assert.
			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.True(task.IsFaulted);
			Assert.NotNull(task.Exception);
			Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
		}

		private readonly TaskScheduler _taskScheduler = new SynchronousTaskScheduler();
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();
	}
}