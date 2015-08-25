// Sharp Essentials
// Copyright 2015 Matthew Hamilton - matthamilton@live.com
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
using System.Threading;
using Xunit.Sdk;

namespace SharpEssentials.Testing
{
    /// <summary>
	/// Sets the current synchronization context to an instance of the specified type.
	/// </summary>
	public class SynchronizationContextAttribute : BeforeAfterTestAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizationContextAttribute"/> class.
        /// </summary>
        /// <param name="contextType">The specific synchronization context type.</param>
        public SynchronizationContextAttribute(Type contextType)
        {
            if (contextType == null)
                throw new ArgumentNullException(nameof(contextType));

            if (!typeof(SynchronizationContext).IsAssignableFrom(contextType))
                throw new ArgumentException("Type must derive from SynchronizationContext.", nameof(contextType));

            ContextType = contextType;
        }

        /// <summary>
        /// The specific synchronization context type.
        /// </summary>
        public Type ContextType { get; set; }

        /// <summary>
        /// Stores the current synchronization context and sets the current
        /// contexts.
        /// </summary>
        /// <param name="methodUnderTest"></param>
        public override void Before(MethodInfo methodUnderTest)
        {
            _originalContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext((SynchronizationContext)Activator.CreateInstance(ContextType));
        }

        /// <summary>
        /// Restores the original synchronization context.
        /// </summary>
        /// <param name="methodUnderTest"></param>
        public override void After(MethodInfo methodUnderTest)
        {
            SynchronizationContext.SetSynchronizationContext(_originalContext);
        }

        private SynchronizationContext _originalContext;
    }
}