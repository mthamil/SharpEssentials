using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using SharpEssentials.Chronology;
using SharpEssentials.InputOutput;
using Xunit;

namespace SharpEssentials.Tests.Unit.SharpEssentials.InputOutput
{
    public class DirectoryMonitorTests
    {
        public DirectoryMonitorTests()
        {
            var timerFactory = new Func<ITimer>(() =>
            {
                var timer = new Mock<ITimer>();
                _timers.Add(timer);
                return timer.Object;
            });

            _underTest = new DirectoryMonitor(_watcher.Object, timerFactory)
            {
                FileCreationWaitTimeout = TimeSpan.FromSeconds(2)
            };
        }

        [Fact]
        public void Test_Filter()
        {
            // Arrange.
            _watcher.SetupProperty(w => w.Filter);

            // Act.
            _underTest.Filter = "*.exe";

            // Assert.
            _watcher.VerifySet(w => w.Filter = "*.exe");
            Assert.Equal("*.exe", _underTest.Filter);
        }

        [Fact]
        public void Test_StartMonitoring()
        {
            // Arrange.
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            // Act.
            _underTest.StartMonitoring(directory);

            // Assert.
            _watcher.VerifySet(w => w.Path = directory.FullName);
            _watcher.VerifySet(w => w.EnableRaisingEvents = true);
            Assert.Equal(directory, _underTest.MonitoredDirectory);
        }

        [Fact]
        public void Test_RestartMonitoring()
        {
            // Arrange.
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            _underTest.StartMonitoring(directory);
            _underTest.StopMonitoring();

            // Act.
            _underTest.RestartMonitoring();

            // Assert.
            _watcher.VerifySet(w => w.Path = directory.FullName);
            _watcher.VerifySet(w => w.EnableRaisingEvents = true);
        }

        [Fact]
        public void Test_RestartMonitoring_NeverStarted()
        {
            // Act/Assert.
            Assert.Throws<InvalidOperationException>(() => _underTest.RestartMonitoring());
        }

        [Fact]
        public void Test_StopMonitoring()
        {
            // Act.
            _underTest.StopMonitoring();

            // Assert.
            _watcher.VerifySet(w => w.EnableRaisingEvents = false);
        }

        [Fact]
        public void Test_Watcher_Created()
        {
            using (var temp = new TemporaryFile().Touch())
            {
                // Arrange.
                var createdArgs = new List<FileSystemEventArgs>();
                void CreatedHandler(object o, FileSystemEventArgs e) => createdArgs.Add(e);
                _underTest.Created += CreatedHandler;

                // Act.
                _watcher.Raise(w => w.Created += null, new FileSystemEventArgs(WatcherChangeTypes.Created, temp.File.Directory.FullName, temp.File.Name));

                // Assert.
                Assert.Single(_timers);
                _timers.Single().VerifySet(t => t.Interval = TimeSpan.FromSeconds(2));
                _timers.Single().Verify(t => t.Restart(temp.File.FullName));

                // Act.
                _timers.Single().Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now, temp.File.FullName));

                // Assert.
                Assert.Single(createdArgs);
                Assert.Equal(temp.File.FullName, createdArgs.Single().FullPath);
                _timers.Single().Verify(t => t.TryStop());
            }
        }

        [Fact]
        public void Test_Watcher_Created_ThenChanged()
        {
            using (var temp = new TemporaryFile().Touch())
            {
                // Arrange.
                var createdArgs = new List<FileSystemEventArgs>();
                void CreatedHandler(object o, FileSystemEventArgs e) => createdArgs.Add(e);
                _underTest.Created += CreatedHandler;

                _watcher.Raise(w => w.Created += null, new FileSystemEventArgs(WatcherChangeTypes.Created, temp.File.Directory.FullName, temp.File.Name));

                // Act.
                _watcher.Raise(w => w.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, temp.File.Directory.FullName, temp.File.Name));

                // Assert.
                Assert.Single(_timers);
                _timers.Single().VerifySet(t => t.Interval = TimeSpan.FromSeconds(2));
                _timers.Single().Verify(t => t.Restart(temp.File.FullName), Times.Exactly(2));

                // Act.
                _timers.Single().Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now, temp.File.FullName));

                // Assert.
                Assert.Single(createdArgs);
                Assert.Equal(temp.File.FullName, createdArgs.Single().FullPath);
                _timers.Single().Verify(t => t.TryStop());
            }
        }

        [Fact]
        public void Test_Watcher_Created_ThenDeleted()
        {
            using (var temp = new TemporaryFile().Touch())
            {
                // Arrange.
                var createdArgs = new List<FileSystemEventArgs>();
                void CreatedHandler(object o, FileSystemEventArgs e) => createdArgs.Add(e);
                _underTest.Created += CreatedHandler;

                _watcher.Raise(w => w.Created += null, new FileSystemEventArgs(WatcherChangeTypes.Created, temp.File.Directory.FullName, temp.File.Name));

                // Act.
                _watcher.Raise(w => w.Deleted += null, new FileSystemEventArgs(WatcherChangeTypes.Deleted, temp.File.Directory.FullName, temp.File.Name));

                // Assert.
                Assert.Single(_timers);
                _timers.Single().Verify(t => t.TryStop());

                // Act.
                _timers.Single().Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now, temp.File.FullName));

                // Assert.
                Assert.Empty(createdArgs);
            }
        }

        [Fact]
        public void Test_Watcher_Deleted()
        {
            // Arrange.
            FileSystemEventArgs deleteArgs = null;
            void DeletedHandler(object o, FileSystemEventArgs e) => deleteArgs = e;
            _underTest.Deleted += DeletedHandler;

            // Act.
            _watcher.Raise(w => w.Deleted += null, new FileSystemEventArgs(WatcherChangeTypes.Deleted, "Dir", "File"));

            // Assert.
            Assert.NotNull(deleteArgs);
            Assert.Equal(@"Dir\File", deleteArgs.FullPath);
        }

        [Fact]
        public void Test_Watcher_Changed()
        {
            // Arrange.
            FileSystemEventArgs changedArgs = null;
            void ChangedHandler(object o, FileSystemEventArgs e) => changedArgs = e;
            _underTest.Changed += ChangedHandler;

            // Act.
            _watcher.Raise(w => w.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, "Dir", "File"));

            // Assert.
            Assert.NotNull(changedArgs);
            Assert.Equal(@"Dir\File", changedArgs.FullPath);
        }

        [Fact]
        public void Test_Watcher_Renamed()
        {
            // Arrange.
            RenamedEventArgs renameArgs = null;
            void RenamedHandler(object o, RenamedEventArgs e) => renameArgs = e;
            _underTest.Renamed += RenamedHandler;

            // Act.
            _watcher.Raise(w => w.Renamed += null, new RenamedEventArgs(WatcherChangeTypes.Renamed, "Dir", "New", "Old"));

            // Assert.
            Assert.NotNull(renameArgs);
            Assert.Equal(@"Dir\New", renameArgs.FullPath);
            Assert.Equal(@"Dir\Old", renameArgs.OldFullPath);
        }

        [Fact]
        public void Test_Dispose()
        {
            // Arrange.
            FileSystemEventArgs createArgs = null;
            void CreatedHandler(object o, FileSystemEventArgs e) => createArgs = e;
            _underTest.Created += CreatedHandler;

            FileSystemEventArgs deleteArgs = null;
            void DeletedHandler(object o, FileSystemEventArgs e) => deleteArgs = e;
            _underTest.Deleted += DeletedHandler;

            FileSystemEventArgs changedArgs = null;
            void ChangedHandler(object o, FileSystemEventArgs e) => changedArgs = e;
            _underTest.Changed += ChangedHandler;

            RenamedEventArgs renameArgs = null;
            void RenamedHandler(object o, RenamedEventArgs e) => renameArgs = e;
            _underTest.Renamed += RenamedHandler;

            // Act.
            _underTest.Dispose();

            _watcher.Raise(w => w.Created += null, new FileSystemEventArgs(WatcherChangeTypes.Created, "Dir", "File"));
            _watcher.Raise(w => w.Deleted += null, new FileSystemEventArgs(WatcherChangeTypes.Deleted, "Dir", "File"));
            _watcher.Raise(w => w.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, "Dir", "File"));
            _watcher.Raise(w => w.Renamed += null, new RenamedEventArgs(WatcherChangeTypes.Renamed, "Dir", "New", "Old"));

            // Assert.
            Assert.Null(createArgs);
            Assert.Null(deleteArgs);
            Assert.Null(changedArgs);
            Assert.Null(renameArgs);

            _watcher.Verify(w => w.Dispose());
        }

        private readonly DirectoryMonitor _underTest;

        private readonly Mock<IFileSystemWatcher> _watcher = new Mock<IFileSystemWatcher>();
        private readonly IList<Mock<ITimer>> _timers = new List<Mock<ITimer>>(); 
    }
}