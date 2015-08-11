// Sharp Essentials
// Copyright 2014 Matthew Hamilton - matthamilton@live.com
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
// 
using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using SharpEssentials.Reflection;

namespace SharpEssentials.Testing
{
	/// <summary>
	/// Proxy that records method invocations.
	/// </summary>
	public class MethodRecorder<T> : RealProxy
	{
		/// <summary>
		/// Creates a new interceptor that records method invocations.
		/// </summary>
		public MethodRecorder()
			: base(typeof(T))
		{
			_proxy = new Lazy<T>(() => (T)base.GetTransparentProxy());
		}

		/// <summary>
		/// The underlying proxy.
		/// </summary>
		public T Proxy => _proxy.Value;

	    /// <summary>
		/// The most recent invocation made on the proxy.
		/// </summary>
		public IMethodCallMessage LastInvocation { get; private set; }

		/// <see cref="RealProxy.Invoke"/>
		public override IMessage Invoke(IMessage msg)
		{
			var methodCall = msg as IMethodCallMessage;
			LastInvocation = methodCall;

			object returnValue = null;
			var method = methodCall.MethodBase as MethodInfo;
			if (method != null)
				returnValue = method.ReturnType.GetDefaultValue();

			return new ReturnMessage(returnValue, new object[0], 0, methodCall.LogicalCallContext, methodCall);
		}

		private readonly Lazy<T> _proxy;
	}
}
