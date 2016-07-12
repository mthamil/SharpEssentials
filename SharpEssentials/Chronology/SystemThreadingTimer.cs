// Sharp Essentials
// Copyright 2016 Matthew Hamilton - matthamilton@live.com
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
using System.Threading;

namespace SharpEssentials.Chronology
{
    /// <summary>
    /// A timer based on <see cref="System.Threading.Timer"/>.
    /// </summary>
    public class SystemThreadingTimer : TimerBase, IDisposable
    {
        /// <summary>
        /// Initializes a new <see cref="SystemThreadingTimer"/>.
        /// </summary>
        public SystemThreadingTimer()
        {
            _timer = new Timer(_ => OnElapsed(DateTime.Now, _state));
        }

        /// <see cref="ITimer.TryStart"/>
        public override bool TryStart(object state = null)
        {
            lock (SyncObject)
            {
                if (_started)
                    return false;

                var changed = _timer.Change(Interval, Interval);
                if (changed)
                {
                    _state = state;
                    _started = true;
                }
                
                return changed;
            }
        }

        /// <see cref="ITimer.TryStop"/>
        public override bool TryStop()
        {
            lock (SyncObject)
            {
                if (!_started)
                    return false;

                var changed = _timer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
                if (changed)
                    _started = false;

                return changed;
            }
        }

        /// <see cref="ITimer.Started"/>
        public override bool Started => _started;

        /// <see cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            lock (SyncObject)
            {
                _timer.Dispose();
            }
        }

        private readonly Timer _timer;
        private object _state;
        private bool _started;
    }
}