// Sharp Essentials
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
using System.Collections.Concurrent;
using System.IO;
using SharpEssentials.Chronology;

namespace SharpEssentials.InputOutput
{
    /// <summary>
    /// Monitors a directory for changes.  Provides
    /// a higher-level of abstraction over a FileSystemWatcher.
    /// </summary>
    public class DirectoryMonitor : DisposableBase, IDirectoryMonitor
    {
        /// <summary>
        /// Initializes a new <see cref="DirectoryMonitor"/>.
        /// </summary>
        /// <param name="fileSystemWatcher">The file system watcher to use</param>
        /// <param name="timerFactory">Creates timers</param>
        public DirectoryMonitor(IFileSystemWatcher fileSystemWatcher, Func<ITimer> timerFactory)
        {
            _fileSystemWatcher = fileSystemWatcher ?? throw new ArgumentNullException(nameof(fileSystemWatcher));
            _timerFactory = timerFactory ?? throw new ArgumentNullException(nameof(timerFactory));

            _fileSystemWatcher.Created += FileSystemWatcher_Created;
            _fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
            _fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            _fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
        }

        /// <see cref="IDirectoryMonitor.Filter"/>
        public string Filter 
        {
            get => _fileSystemWatcher.Filter;
            set => _fileSystemWatcher.Filter = value;
        }

        /// <see cref="IDirectoryMonitor.MonitoredDirectory"/>
        public DirectoryInfo MonitoredDirectory { get; private set; }

        /// <see cref="IDirectoryMonitor.StartMonitoring"/>
        public void StartMonitoring(DirectoryInfo directory)
        {
            _fileSystemWatcher.Path = directory.FullName;
            MonitoredDirectory = directory;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        /// <see cref="IDirectoryMonitor.RestartMonitoring"/>
        public void RestartMonitoring()
        {
            if (MonitoredDirectory == null)
                throw new InvalidOperationException("No directory was previously monitored.");

            StartMonitoring(MonitoredDirectory);
        }

        /// <see cref="IDirectoryMonitor.StopMonitoring"/>
        public void StopMonitoring()
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
        }

        /// <see cref="IDirectoryMonitor.Changed"/>
        public event EventHandler<FileSystemEventArgs> Changed;

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            ITimer timer;
            if (_fileCreationTimers.TryGetValue(e.FullPath, out timer))
                timer.Restart(e.FullPath);	// The created file is still changing, reset the timer.

            OnChanged(e);
        }

        private void OnChanged(FileSystemEventArgs args)
        {
            Changed?.Invoke(this, args);
        }

        /// <see cref="IDirectoryMonitor.Created"/>
        public event EventHandler<FileSystemEventArgs> Created;

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            _fileCreationTimers.GetOrAdd(e.FullPath, path =>
            {
                var timer = _timerFactory();
                timer.Interval = FileCreationWaitTimeout;
                timer.Elapsed += FileTimer_Elapsed;
                timer.Restart(path);
                return timer;
            });
        }

        private void OnCreated(FileSystemEventArgs args)
        {
            Created?.Invoke(this, args);
        }

        private void FileTimer_Elapsed(object sender, TimerElapsedEventArgs e)
        {
            var path = (string)e.State;
            if (_fileCreationTimers.TryRemove(path, out ITimer creationTimer))
            {
                creationTimer.TryStop();
                creationTimer.Elapsed -= FileTimer_Elapsed;
                if (File.Exists(path))
                    OnCreated(new FileSystemEventArgs(WatcherChangeTypes.Created, Path.GetDirectoryName(path), Path.GetFileName(path)));
            }
        }

        /// <see cref="IDirectoryMonitor.Deleted"/>
        public event EventHandler<FileSystemEventArgs> Deleted;

        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (_fileCreationTimers.TryRemove(e.FullPath, out ITimer creationTimer))
            {
                creationTimer.TryStop();
                creationTimer.Elapsed -= FileTimer_Elapsed;
            }

            OnDeleted(e);
        }

        private void OnDeleted(FileSystemEventArgs args)
        {
            Deleted?.Invoke(this, args);
        }

        /// <see cref="IDirectoryMonitor.Renamed"/>
        public event EventHandler<RenamedEventArgs> Renamed;

        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            OnRenamed(e);
        }

        private void OnRenamed(RenamedEventArgs args)
        {
            Renamed?.Invoke(this, args);
        }

        /// <summary>
        /// The maximum amount of time to wait for a file to be created after receiving
        /// the Created event.
        /// </summary>
        public TimeSpan FileCreationWaitTimeout { get; set; }

        #region DisposableBase Members

        /// <see cref="DisposableBase.OnDisposing"/>
        protected override void OnDisposing()
        {
            _fileSystemWatcher.Created -= FileSystemWatcher_Created;
            _fileSystemWatcher.Deleted -= FileSystemWatcher_Deleted;
            _fileSystemWatcher.Changed -= FileSystemWatcher_Changed;
            _fileSystemWatcher.Renamed -= FileSystemWatcher_Renamed;
        }

        /// <see cref="DisposableBase.OnDispose"/>
        protected override void OnDispose()
        {
            _fileSystemWatcher.Dispose();
        }

        #endregion

        /// <summary>
        /// Timers used to track file changes.
        /// </summary>
        private readonly ConcurrentDictionary<string, ITimer> _fileCreationTimers = new ConcurrentDictionary<string, ITimer>();

        private readonly IFileSystemWatcher _fileSystemWatcher;
        private readonly Func<ITimer> _timerFactory;
    }
}