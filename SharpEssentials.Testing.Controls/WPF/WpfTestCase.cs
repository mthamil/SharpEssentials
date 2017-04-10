// Sharp Essentials
// Copyright 2017 Matthew Hamilton - matthamilton@live.com
// Copyright 2015 Outercurve Foundation
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace SharpEssentials.Testing.Controls.WPF
{
    /// <summary>
    /// Wraps test cases for FactAttribute and TheoryAttribute so the test case runs on the WPF Dispatcher.
    /// </summary>
    /// <remarks>
    /// See: https://github.com/xunit/samples.xunit/tree/master/STAExamples
    /// </remarks>
    [DebuggerDisplay(@"\{ class = {TestMethod.TestClass.Class.Name}, method = {TestMethod.Method.Name}, display = {DisplayName}, skip = {SkipReason} \}")]
    public class WpfTestCase : LongLivedMarshalByRefObject, IXunitTestCase
    {
        private IXunitTestCase _testCase;

        /// <summary>
        /// Initializes a new <see cref="WpfTestCase"/>.
        /// </summary>
        /// <param name="testCase">The test case to wrap.</param>
        public WpfTestCase(IXunitTestCase testCase)
        {
            _testCase = testCase;
        }

        /// <summary/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer", error: true)]
        public WpfTestCase() { }

        /// <summary>
        /// See <see cref="IXunitTestCase.Method"/>.
        /// </summary>
        public IMethodInfo Method => _testCase.Method;

        /// <summary>
        /// See <see cref="IXunitTestCase.RunAsync"/>
        /// </summary>
        public Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink,
                                         IMessageBus messageBus,
                                         object[] constructorArguments,
                                         ExceptionAggregator aggregator,
                                         CancellationTokenSource cancellationTokenSource)
        {
            var tcs = new TaskCompletionSource<RunSummary>();
            var thread = new Thread(() =>
            {
                try
                {
                    // Set up the SynchronizationContext so that any awaits
                    // resume on the Dispatcher thread as they would in a GUI app.
                    SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());

                    // Start off the test method.
                    var testCaseTask = _testCase.RunAsync(diagnosticMessageSink, messageBus, constructorArguments, aggregator, cancellationTokenSource);

                    // Arrange to pump messages to execute any async work associated with the test.
                    var frame = new DispatcherFrame();
                    Task.Run(async () =>
                    {
                        try
                        {
                            await testCaseTask;
                        }
                        finally
                        {
                            // The test case's execution is done. Terminate the message pump.
                            frame.Continue = false;
                        }
                    });
                    Dispatcher.PushFrame(frame);

                    // Report the result back to the Task we returned earlier.
                    CopyTaskResultFrom(tcs, testCaseTask);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return tcs.Task;
        }

        /// <summary>
        /// See <see cref="ITestCase.DisplayName"/>.
        /// </summary>
        public string DisplayName => _testCase.DisplayName;

        /// <summary>
        /// See <see cref="ITestCase.SkipReason"/>.
        /// </summary>
        public string SkipReason => _testCase.SkipReason;

        /// <summary>
        /// See <see cref="ITestCase.SourceInformation"/>.
        /// </summary>
        public ISourceInformation SourceInformation
        {
            get => _testCase.SourceInformation;
            set => _testCase.SourceInformation = value;
        }

        /// <summary>
        /// See <see cref="ITestCase.TestMethod"/>.
        /// </summary>
        public ITestMethod TestMethod => _testCase.TestMethod;

        /// <summary>
        /// See <see cref="ITestCase.TestMethodArguments"/>.
        /// </summary>
        public object[] TestMethodArguments => _testCase.TestMethodArguments;

        /// <summary>
        /// See <see cref="ITestCase.Traits"/>.
        /// </summary>
        public Dictionary<string, List<string>> Traits => _testCase.Traits;

        /// <summary>
        /// See <see cref="ITestCase.UniqueID"/>.
        /// </summary>
        public string UniqueID => _testCase.UniqueID;

        /// <summary>
        /// See <see cref="IXunitSerializable.Deserialize"/>.
        /// </summary>
        public void Deserialize(IXunitSerializationInfo info)
        {
            _testCase = info.GetValue<IXunitTestCase>("InnerTestCase");
        }

        /// <summary>
        /// See <see cref="IXunitSerializable.Serialize"/>.
        /// </summary>
        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("InnerTestCase", _testCase);
        }

        private static void CopyTaskResultFrom<T>(TaskCompletionSource<T> tcs, Task<T> template)
        {
            if (tcs == null)
                throw new ArgumentNullException(nameof(tcs));
            if (template == null)
                throw new ArgumentNullException(nameof(template));
            if (!template.IsCompleted)
                throw new ArgumentException("Task must be completed first.", nameof(template));

            if (template.IsFaulted)
                tcs.SetException(template.Exception);
            else if (template.IsCanceled)
                tcs.SetCanceled();
            else
                tcs.SetResult(template.Result);
        }
    }
}