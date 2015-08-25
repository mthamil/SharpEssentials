using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace SharpEssentials.Testing.Controls.WPF
{
    public abstract class WpfTestDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IXunitTestCaseDiscoverer _innerDiscoverer;

        protected WpfTestDiscoverer(IXunitTestCaseDiscoverer innerDiscoverer)
        {
            _innerDiscoverer = innerDiscoverer;
        }

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            return _innerDiscoverer.Discover(discoveryOptions, testMethod, factAttribute)
                                   .Select(testCase => new WpfTestCase(testCase));
        }
    }
}