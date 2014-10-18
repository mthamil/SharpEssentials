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
using System.Threading;
using SharpEssentials.Concurrency;
using Xunit;

namespace SharpEssentials.Testing
{
	/// <summary>
	/// Sets the current synchronization context to a context that is synchronous.
	/// </summary>
	public class SynchronousAttribute : BeforeAfterTestAttribute
	{
		/// <summary>
		/// Stores the current synchronization context and sets the current
		/// context to be synchronous.
		/// </summary>
		/// <param name="methodUnderTest"></param>
		public override void Before(System.Reflection.MethodInfo methodUnderTest)
		{
			_originalContext = SynchronizationContext.Current;
			SynchronizationContext.SetSynchronizationContext(new SynchronousSynchronizationContext());
		}

		/// <summary>
		/// Restores the original synchronization context.
		/// </summary>
		/// <param name="methodUnderTest"></param>
		public override void After(System.Reflection.MethodInfo methodUnderTest)
		{
			SynchronizationContext.SetSynchronizationContext(_originalContext);
		}

		private SynchronizationContext _originalContext;
	}
}