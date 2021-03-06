﻿using System;
using System.Linq;
using System.Threading.Tasks;
using SharpEssentials.Concurrency;
using SharpEssentials.Testing;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Concurrency
{
    public class TasksTests
    {
        [Fact]
        public void Test_FromCanceled()
        {
            // Act.
            var task = Tasks.FromCanceled<string>();

            // Assert.
            Assert.True(task.IsCompleted);
            Assert.True(task.IsCanceled);
            Assert.False(task.IsFaulted);
            Assert.Equal(TaskStatus.Canceled, task.Status);
        }

        [Fact]
        public void Test_FromCanceled_NoResult()
        {
            // Act.
            var task = Tasks.FromCanceled();

            // Assert.
            Assert.True(task.IsCompleted);
            Assert.True(task.IsCanceled);
            Assert.False(task.IsFaulted);
            Assert.Equal(TaskStatus.Canceled, task.Status);
        }

        [Fact]
        public void Test_FromException_NoExistingException()
        {
            // Act.
            var task = Tasks.FromException<InvalidOperationException>();

            // Assert.
            Assert.True(task.IsCompleted);
            Assert.False(task.IsCanceled);
            Assert.True(task.IsFaulted);
            Assert.Equal(TaskStatus.Faulted, task.Status);
            Assert.NotNull(task.Exception);
            Assert.IsType<InvalidOperationException>(task.Exception.InnerException);
        }

        [Fact]
        public void Test_FromExceptions()
        {
            // Act.
            var task = Tasks.FromExceptions<int>(new InvalidOperationException(), new Exception(), new FormatException());

            // Assert.
            Assert.True(task.IsCompleted);
            Assert.False(task.IsCanceled);
            Assert.True(task.IsFaulted);
            Assert.Equal(TaskStatus.Faulted, task.Status);
            Assert.NotNull(task.Exception);
            AssertThat.SequenceEqual(
                new [] { typeof(InvalidOperationException), typeof(Exception), typeof(FormatException) }, 
                task.Exception.InnerExceptions.Select(e => e.GetType()));
        }

        [Fact]
        public void Test_FromExceptions_NoResult()
        {
            // Act.
            var task = Tasks.FromExceptions(new InvalidOperationException(), new Exception(), new FormatException());

            // Assert.
            Assert.True(task.IsCompleted);
            Assert.False(task.IsCanceled);
            Assert.True(task.IsFaulted);
            Assert.Equal(TaskStatus.Faulted, task.Status);
            Assert.NotNull(task.Exception);
            AssertThat.SequenceEqual(
                new[] { typeof(InvalidOperationException), typeof(Exception), typeof(FormatException) }, 
                task.Exception.InnerExceptions.Select(e => e.GetType()));
        }

        [Fact]
        public void Test_Empty()
        {
            // Act.
            var task = Tasks.Empty<string>();

            // Assert.
            Assert.True(task.IsCompleted);
            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            Assert.Empty(task.Result);
        }
    }
}