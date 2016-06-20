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
using System.Windows;

namespace SharpEssentials.Controls.Weak
{
    public static class WeakEventManagerExtensions
    {
        public static void AddWeakHandler<TSource, TEventArgs>(this TSource source, string eventName,
                                                               EventHandler<TEventArgs> handler) where TEventArgs : EventArgs
        {
            WeakEventManager<TSource, TEventArgs>.AddHandler(source, eventName, handler);
        }

        public static void RemoveWeakHandler<TSource, TEventArgs>(this TSource source, string eventName,
                                                                  EventHandler<TEventArgs> handler) where TEventArgs : EventArgs
        {
            WeakEventManager<TSource, TEventArgs>.RemoveHandler(source, eventName, handler);
        }
    }
}