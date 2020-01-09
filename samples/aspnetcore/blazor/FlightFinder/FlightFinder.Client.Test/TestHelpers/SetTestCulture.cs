using System.Globalization;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: Xunit.TestFramework("FlightFinder.Client.Test.TestHelpers.SetTestCulture", "FlightFinder.Client.Test")]

namespace FlightFinder.Client.Test.TestHelpers
{
    public class SetTestCulture : XunitTestFramework
    {
        const string culture = "en-US";

        public SetTestCulture(IMessageSink messageSink) : base(messageSink)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
        }
    }
}
