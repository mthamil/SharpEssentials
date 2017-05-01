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

using System.Reflection;
using SharpEssentials.Reflection;

namespace SharpEssentials.Testing
{
    /// <summary>
    /// Proxy that records method invocations.
    /// </summary>
    public class MethodRecorder<T> : DispatchProxy, IMethodRecorder
    {
        /// <summary>
        /// Creates a new interceptor that records method invocations.
        /// </summary>
        public static T Create() => Create<T, MethodRecorder<T>>();

        /// <summary>
        /// The most recent invocation made on the proxy.
        /// </summary>
        public MethodInfo LastInvocation { get; private set; }

        /// <summary>
        /// <see cref="DispatchProxy.Invoke"/>
        /// </summary>
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            LastInvocation = targetMethod;
            var returnValue = targetMethod.ReturnType.GetDefaultValue();
            return returnValue;
        }
    }

    internal interface IMethodRecorder
    {
        MethodInfo LastInvocation { get; }
    }
}
