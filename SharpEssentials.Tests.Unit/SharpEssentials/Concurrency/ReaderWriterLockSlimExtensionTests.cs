using System;
using System.Threading;
using SharpEssentials.Concurrency;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.Concurrency
{
	public class ReaderWriterLockSlimExtensionTests : IDisposable
	{
		[Fact]
		public void Test_ReadLock()
		{
			var readLock = _underTest.ReadLock();

			Assert.True(_underTest.IsReadLockHeld);
			Assert.False(_underTest.IsUpgradeableReadLockHeld);
			Assert.False(_underTest.IsWriteLockHeld);

			readLock.Dispose();

			Assert.False(_underTest.IsReadLockHeld);
			Assert.False(_underTest.IsUpgradeableReadLockHeld);
			Assert.False(_underTest.IsWriteLockHeld);
		}

		[Fact]
		public void Test_WriteLock()
		{
			var writeLock = _underTest.WriteLock();

			Assert.False(_underTest.IsReadLockHeld);
			Assert.False(_underTest.IsUpgradeableReadLockHeld);
			Assert.True(_underTest.IsWriteLockHeld);

			writeLock.Dispose();

			Assert.False(_underTest.IsReadLockHeld);
			Assert.False(_underTest.IsUpgradeableReadLockHeld);
			Assert.False(_underTest.IsWriteLockHeld);
		}

		[Fact]
		public void Test_UpgradeableReadLock()
		{
			var upgradeableReadLock = _underTest.UpgradeableReadLock();

			Assert.False(_underTest.IsReadLockHeld);
			Assert.True(_underTest.IsUpgradeableReadLockHeld);
			Assert.False(_underTest.IsWriteLockHeld);

			upgradeableReadLock.Dispose();

			Assert.False(_underTest.IsReadLockHeld);
			Assert.False(_underTest.IsUpgradeableReadLockHeld);
			Assert.False(_underTest.IsWriteLockHeld);
		}

		[Fact]
		public void Test_UpgradeableReadLock_Upgrade()
		{
			using (_underTest.UpgradeableReadLock())
			{
				var upgradedLock = _underTest.WriteLock();

				Assert.False(_underTest.IsReadLockHeld);
				Assert.True(_underTest.IsUpgradeableReadLockHeld);
				Assert.True(_underTest.IsWriteLockHeld);

				upgradedLock.Dispose();

				Assert.False(_underTest.IsReadLockHeld);
				Assert.True(_underTest.IsUpgradeableReadLockHeld);
				Assert.False(_underTest.IsWriteLockHeld);
			}
		}

		private readonly ReaderWriterLockSlim _underTest = new ReaderWriterLockSlim();

		public void Dispose()
		{
			_underTest.Dispose();
		}
	}
}
